using System;
using System.Collections.Generic;
using LLama.Experimental.Abstractions;
using Microsoft.Extensions.Logging;
using LLama.Experimental.Common;
using LLama.Experimental.Utils;
using LLama.Extensions;
using System.Linq;
using System.Diagnostics;
using LLama.Experimental.Config;

namespace LLama.Experimental
{
    /// <summary>
    /// An LLM engine that receives requests and generates texts.
    /// 
    /// It receives requests
    /// from clients and generates texts from the LLM.It includes a tokenizer, a
    /// language model, and GPU memory space allocated for intermediate states(aka KV cache). 
    /// This class utilizes iteration-level scheduling and efficient memory management 
    /// to maximize the serving throughput.
    /// </summary>
    internal sealed class LLMEngine
    {
        private ILogger? _logger;

        private IdCounter _seqCounter;

        public Scheduler Scheduler { get; }

        public IModelRunner ModelRunner { get; }

        public ITokenizer Tokenizer { get; set; }

        /// <summary>
        /// Gets the number of unfinished requests.
        /// </summary>
        public int NumUnfinishedRequests => Scheduler.GetNumUnfinishedSeqGroups();

        /// <summary>
        /// Returns True if there are unfinished requests.
        /// </summary>
        public bool HasUnfinishedRequests => Scheduler.HasUnfinishedSeqs();

        public LLMEngine(SchedulerConfig schedulerConfig, IModelRunner modelRunner, ITokenizer tokenizer, ILogger? logger = null)
        {
            _seqCounter = new();
            Scheduler = new Scheduler(schedulerConfig, new KvCacheConfig(), logger);
            Tokenizer = tokenizer;
            _logger = logger;
            ModelRunner = modelRunner;
        }

        /// <summary>
        /// Performs one decoding iteration and returns newly generated results.
        /// 
        /// Details:
        ///     - Step 1: Schedules the sequences to be executed in the next
        /// iteration and the token blocks to be swapped in/out/copy.
        /// 
        ///         - Depending on the scheduling policy,
        /// sequences may be `preempted/reordered`.
        ///         - A Sequence Group(SG) refer to a group of sequences
        /// that are generated from the same prompt.
        /// 
        ///     - Step 2: Calls the distributed executor to execute the model.
        ///     - Step 3: Processes the model output. This mainly includes:
        /// 
        ///          - Decodes the relevant outputs.
        ///          - Updates the scheduled sequence groups with model outputs
        /// based on its `sampling parameters` (`use_beam_search` or not).
        ///         - Frees the finished sequence groups.
        /// 
        ///     - Finally, it creates and returns the newly generated results.
        /// </summary>
        /// <returns></returns>
        public List<RequestOutput> Step()
        {
            var (seqGroupMetadataList, schedulerOutputs) = Scheduler.Schedule();
            var output = !schedulerOutputs.IsEmpty ? ModelRunner.ExecuteModel(seqGroupMetadataList) : new SamplerOutput([]);
            return ProcessModelOutputs(output, schedulerOutputs);
        }

        /// <summary>
        /// Add a request to the engine's request pool.
        /// 
        /// The request is added to the request pool and will be processed by the
        /// scheduler as `engine.step()` is called.The exact scheduling policy is
        /// determined by the scheduler.
        /// </summary>
        /// <param name="requestId">The unique ID of the request.</param>
        /// <param name="prompt">The prompt string. Can be Null or empty if prompt_token_ids is provided.</param>
        /// <param name="samplingMethod">The sampling parameters for text generation.</param>
        /// <param name="stoppingCriteria">The stopping criteria to decide whether the generation should be stopped.</param>
        /// <param name="promptTokenIds">The token IDs of the prompt. If Null, we use the tokenizer to convert the prompts to token IDs.</param>
        /// <param name="arrivalTime">The arrival time of the request. If Null, we use the current monotonic time.</param>
        public void AddRequest(string requestId, string? prompt, ISamplingMethod samplingMethod, IStoppingCriteria stoppingCriteria, IList<int>? promptTokenIds = null, DateTime? arrivalTime = null)
        {
            arrivalTime ??= DateTime.Now;
            if(promptTokenIds is null)
            {
                Debug.Assert(prompt is not null);
                promptTokenIds = Tokenizer.Tokenize(prompt!);
            }
            else if (!string.IsNullOrEmpty(prompt))
            {
                _logger?.LogWarning("Both prompt and prompt_token_ids are provided. The prompt will be ignored.");
            }

            var seqId = _seqCounter.Next();
            var seq = new Sequence(seqId, prompt, promptTokenIds);
            var seqGroup = new SequenceGroup(requestId, [seq], samplingMethod, stoppingCriteria, arrivalTime.Value);

            // Add the sequence group to the scheduler.
            Scheduler.AddSeqGroup(seqGroup);
        }

        private List<RequestOutput> ProcessModelOutputs(SamplerOutput outputs, SchedulerOutputs schedulerOutputs)
        {
            var now = DateTime.Now;
            // Update the scheduled sequence groups with the model outputs.
            var scheduledSeqGroups = schedulerOutputs.ScheduledSeqGroups;
            Debug.Assert(scheduledSeqGroups.Count() == outputs.Count);
            int i = 0;
            foreach(var scheduledSeqGroup in scheduledSeqGroups)
            {
                var output = outputs[i];
                var seqGroup = scheduledSeqGroup.SeqGroup;
                seqGroup.UpdateNumComputedTokens(scheduledSeqGroup.TokenChunkSize);
                ProcessSequenceGroupOutputs(seqGroup, output);
                i++;
            }

            // Free the finished sequence groups.
            Scheduler.FreeFinishedSeqGroups();

            // Create the outputs.
            List<RequestOutput> requestOutputs = new();
            foreach(var scheduledSeqGroup in scheduledSeqGroups)
            {
                var seqGroup = scheduledSeqGroup.SeqGroup;
                seqGroup.MaybeSetFirstTokenTime(now);
                requestOutputs.Add(RequestOutput.FromSeqGroup(seqGroup));
            }
            foreach(var seqGroup in schedulerOutputs.IgnoredSeqGroups)
            {
                requestOutputs.Add(RequestOutput.FromSeqGroup(seqGroup));
            }

            // TODO: log stats here.
            return requestOutputs;
        }

        private void ProcessSequenceGroupOutputs(SequenceGroup seqGroup, SequenceGroupOutput outputs)
        {
            // TODO: support using logprobs
            var samples = outputs.Samples;
            var parentSeqs = seqGroup.GetSeqsWithStatus(SequenceStatus.Running);
            var existingFinishedSeqs = seqGroup.GetFinishedSeqs();
            var parentChildDict = parentSeqs.ToDictionary(x => x.Id, _ => new List<SequenceOutput>());
            foreach(var sample in samples)
            {
                parentChildDict[sample.ParentSeqId].Add(sample);
            }
            // List of (child, parent)
            List<(Sequence, Sequence)> childSeqs = new();

            foreach(var parent in parentSeqs)
            {
                var childSamples = parentChildDict[parent.Id];
                if(childSamples.Count == 0)
                {
                    // This parent sequence has no children samples. Remove the parent sequence
                    // from the sequence group since it will not be used in the future iterations.
                    parent.Status = SequenceStatus.FinishAborted;
                    seqGroup.Remove(parent.Id);
                    Scheduler.FreeSeq(parent);
                    continue;
                }
                foreach(var childSample in childSamples.SkipLast(1))
                {
                    var newChildSeqId = _seqCounter.Next();
                    var child = parent.Fork(newChildSeqId);
                    child.AppendToken(childSample.OutputTokenId);
                    childSeqs.Add((child, parent));
                }
                // Continue the parent sequence for the last child sample.
                // We reuse the parent sequence here to reduce redundant memory
                // copies, especially when using non-beam search sampling methods.
                var lastChildSample = childSamples.Last();
                parent.AppendToken(lastChildSample.OutputTokenId);
                childSeqs.Add((parent, parent));
            }

            foreach(var (seq, _) in childSeqs)
            {
                DeTokenizer.DecodeSequenceInplace(seq, Tokenizer, seqGroup.SamplingMethod);
                var stoppingCriteriaOutput = seqGroup.StoppingCriteria.CheckStop(seq);
                seq.Status = stoppingCriteriaOutput.Status;
                seq.StoppingTokenId = stoppingCriteriaOutput.StoppingTokenId;
                seq.StoppingString = stoppingCriteriaOutput.StoppingString;
            }

            // Only implement non beam-search case here now.
            // TODO: deal with beam search.
            {
                // For newly created child sequences, add them to the sequence group.
                foreach(var (seq, parent) in childSeqs)
                {
                    if(seq != parent) // if the reference are not the same
                    {
                        seqGroup.Add(seq);
                    }
                    // TODO: see if we need to do the fork in the scheduler.
                }

                // NOTE: be careful of this logic.
                foreach(var (seq, parent) in childSeqs)
                {
                    if(seq == parent && seq.IsFinished)
                    {
                        Scheduler.FreeSeq(seq);
                    }
                }
                return;
            }
        }
    }
}
