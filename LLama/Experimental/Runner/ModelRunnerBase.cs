using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using LLama.Extensions;

namespace LLama.Experimental.Runner
{
    /// <summary>
    /// A class that provides some commonly used method when running the model.
    /// 
    /// Note that you could certainly not use this helper class and implement <see cref="IModelRunner"/> from scratch.
    /// </summary>
    public abstract class ModelRunnerBase
    {
        protected ModelRunnerInput PrepareInputs(IEnumerable<SequenceGroupMetadata> seqGroupMetadataList)
        {
            Debug.Assert(seqGroupMetadataList.Count() > 0);
            if (seqGroupMetadataList.First().IsPrompt)
            {
                return PreparePrompt(seqGroupMetadataList);
            }
            else
            {
                return PrepareDecode(seqGroupMetadataList);
            }
        }

        protected SamplingMetadata PrepareSample(IEnumerable<SequenceGroupMetadata> seqGroupMetadataList, int[] promptLengths, int[] subqueryLengths)
        {
            // TODO: implement it.
            return null;
        }

        /// <summary>
        /// Prepare input for sequences at prefill stage.
        /// </summary>
        /// <param name="seqGroupMetadataList"></param>
        /// <returns></returns>
        protected ModelRunnerInput PreparePrompt(IEnumerable<SequenceGroupMetadata> seqGroupMetadataList)
        {
            Debug.Assert(seqGroupMetadataList.Count() > 0);
            List<int> inputTokenIds = new();
            List<int> inputPositions = new();
            List<int> sequenceIdMapping = new(); // sequennce id of corresponding tokens
            List<bool> withLogits = new();

            List<int> promptLengths = new();
            List<int> contextLengths = new();
            List<int> subqueryLengths = new();

            foreach(var seqGroupMetadata in seqGroupMetadataList)
            {
                Debug.Assert(seqGroupMetadata.IsPrompt);
                var seqIds = seqGroupMetadata.SeqData.Keys.ToList();
                Debug.Assert(seqIds.Count == 1);
                var seqId = seqIds[0];

                var tokenChunkSize = seqGroupMetadata.TokenChunkSize;
                var seqData = seqGroupMetadata.SeqData[seqId];
                var computedLength = seqData.NumComputedTokens;
                //  We should use `Length` here because in case of preemption it contains output tokens.
                var prefillEnd = Math.Min(seqData.Length, computedLength + tokenChunkSize);
                var prompTokenIds = seqData.TokenIds.Take(prefillEnd).Skip(computedLength);
                var promptLength = prompTokenIds.Count();
                // Right now, the prefill_end is always same as the length of sequence.
                // However, once chunked prefill is introduced, this assumption can be changed.
                Debug.Assert(prefillEnd == seqData.Length);
                promptLengths.Add(promptLength);

                // TODO: check the logic here, related with blocks?

                // actual prompt lens
                contextLengths.Add(computedLength);
                subqueryLengths.Add(promptLength - computedLength);

                inputTokenIds.AddRange(prompTokenIds);
                // NOTE: Here we assume that the first token in the prompt is always the first token in the sequence.
                inputPositions.AddRange(Enumerable.Range(computedLength, prefillEnd));

                // TODO: deal with sliding window here?
                sequenceIdMapping.AddRange(Enumerable.Repeat(seqId, promptLength));

                withLogits.AddRange(Enumerable.Repeat(false, promptLength - 1));
                withLogits.Add(true);
            }

            int maxSubqueryLength = subqueryLengths.Max();
            int maxPromptLength = promptLengths.Max();
            int numPromptTokens = inputTokenIds.Count;
            Debug.Assert(maxSubqueryLength > 0);

            return new ModelRunnerInput(inputTokenIds.ToArray(), inputPositions.ToArray(), sequenceIdMapping.ToArray(), 
                withLogits.ToArray(), promptLengths.ToArray(), subqueryLengths.ToArray());
        }

        /// <summary>
        /// Prepare input for sequences at decode stage.
        /// </summary>
        /// <param name="seqGroupMetadataList"></param>
        /// <returns></returns>
        protected ModelRunnerInput PrepareDecode(IEnumerable<SequenceGroupMetadata> seqGroupMetadataList)
        {
            Debug.Assert(seqGroupMetadataList.Count() > 0);
            List<int> inputTokenIds = new();
            List<int> inputPositions = new();
            List<int> sequenceIdMapping = new(); // sequennce id of corresponding tokens
            List<bool> withLogits = new();

            foreach (var seqGroupMetadata in seqGroupMetadataList)
            {
                Debug.Assert(!seqGroupMetadata.IsPrompt);
                Debug.Assert(seqGroupMetadata.TokenChunkSize == 1);
                var seqIds = seqGroupMetadata.SeqData.Keys.ToList();

                foreach(var seqId in seqIds)
                {
                    var seqData = seqGroupMetadata.SeqData[seqId];
                    var generationToken = seqData.LastTokenId;
                    inputTokenIds.Add(generationToken);

                    var seqLength = seqData.Length;
                    var position = seqLength - 1;
                    inputPositions.Add(position);

                    sequenceIdMapping.Add(seqId);
                    withLogits.Add(true);
                }
            }

            return new ModelRunnerInput(inputTokenIds.ToArray(), inputPositions.ToArray(), 
                sequenceIdMapping.ToArray(), withLogits.ToArray(), [], []);
        }
    }
}
