using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using LLama.Experimental.Config;
using LLama.Experimental.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace LLama.Experimental
{
//#if NET8_0_OR_GREATER
//    [Experimental("LLM")]
//#endif
    public sealed class LLM
    {
        private LLMEngine _engine;

        private IdCounter _counter;

        /// <summary>
        /// Get or set the tokenizer used for this <see cref="LLM"/>.
        /// </summary>
        public ITokenizer Tokenizer
        {
            get => _engine.Tokenizer;
            set => _engine.Tokenizer = value;
        }

        public LLM(IModelRunner modelRunner, ITokenizer tokenizer, SchedulerConfig schedulerConfig, ILogger? logger = null)
        {
            _engine = new LLMEngine(schedulerConfig, modelRunner, tokenizer, logger);
            _counter = new();
        }

        /// <summary>
        /// Generates the completions for the input prompt. If you have multiple inputs, 
        /// please use <see cref="Generate(IEnumerable{string}, ISamplingMethod?, IStoppingCriteria?, ProgressCallback?)"/>, 
        /// instead of calling this method multiple times.
        /// </summary>
        /// <param name="prompt">A prompt string.</param>
        /// <param name="samplingMethod">The sampling parameters for text generation. If null, we use the default sampling parameters.</param>
        /// <param name="stoppingCriteria">
        /// The criteria to control whether a sequence generation should be stopped. If null, we use the default stopping criteria.
        /// </param>
        /// <param name="progressCallback">The callback to get the progress of the generation.</param>
        /// <returns>A list of <see cref="RequestOutput"/> objects containing the generated completions in the same order as the input prompts.</returns>
        public RequestOutput[] Generate(string prompt, ISamplingMethod? samplingMethod = null, 
            IStoppingCriteria? stoppingCriteria = null, ProgressCallback? progressCallback = null)
        {
            return Generate([prompt], samplingMethod, stoppingCriteria, progressCallback);
        }

        /// <summary>
        /// Generates the completions for the input prompt. If you have multiple inputs, 
        /// please use <see cref="Generate(IList{IList{int}}, ISamplingMethod?, IStoppingCriteria?, ProgressCallback?)"/>
        /// instead of calling this method multiple times.
        /// </summary>
        /// <param name="promptTokenIds">Token ids.</param>
        /// <param name="samplingMethod">The sampling parameters for text generation. If null, we use the default sampling parameters.</param>
        /// <param name="stoppingCriteria">
        /// The criteria to control whether a sequence generation should be stopped. If null, we use the default stopping criteria.
        /// </param>
        /// <param name="progressCallback">The callback to get the progress of the generation.</param>
        /// <returns>A list of <see cref="RequestOutput"/> objects containing the generated completions in the same order as the input prompt ids.</returns>
        public RequestOutput[] Generate(IList<int> promptTokenIds, ISamplingMethod? samplingMethod = null, 
            IStoppingCriteria? stoppingCriteria = null, ProgressCallback? progressCallback = null)
        {
            return Generate([promptTokenIds], samplingMethod, stoppingCriteria, progressCallback);
        }

        /// <summary>
        /// Generates the completions for the input prompts. 
        /// This class automatically batches the given prompts, considering
        /// the memory constraint. For the best performance, please put all of your prompts
        /// into a single list and pass it to this method.
        /// </summary>
        /// <param name="prompts">A list of prompts to generate completions for.</param>
        /// <param name="samplingMethod">The sampling parameters for text generation. If null, we use the default sampling parameters.</param>
        /// <param name="stoppingCriteria">
        /// The criteria to control whether a sequence generation should be stopped. If null, we use the default stopping criteria.
        /// </param>
        /// <param name="progressCallback">The callback to get the progress of the generation.</param>
        /// <returns>A list of <see cref="RequestOutput"/> objects containing the generated completions in the same order as the input prompts.</returns>
        public RequestOutput[] Generate(IEnumerable<string> prompts, ISamplingMethod? samplingMethod = null, 
            IStoppingCriteria? stoppingCriteria = null, ProgressCallback? progressCallback = null)
        {
            if(prompts.Count() == 0)
            {
                return [];
            }
            samplingMethod ??= GlobalConfig.DefaultSamplingMethod;
            stoppingCriteria ??= GlobalConfig.DefaultStoppingCriteria;

            // Add requests to the engine.
            foreach(var prompt in prompts)
            {
                AddRequest(prompt, samplingMethod, stoppingCriteria);
            }
            return RunEngine(progressCallback);
        }

        /// <summary>
        /// Generates the completions for the input prompt ids list. 
        /// This class automatically batches the given prompts, considering
        /// the memory constraint. For the best performance, please put all of your prompts
        /// into a single list and pass it to this method.
        /// </summary>
        /// <param name="promptTokenIds">A list of token ids to generate completion for.</param>
        /// <param name="samplingMethod">The sampling parameters for text generation. If null, we use the default sampling parameters.</param>
        /// <param name="stoppingCriteria">
        /// The criteria to control whether a sequence generation should be stopped. If null, we use the default stopping criteria.
        /// </param>
        /// <param name="progressCallback">The callback to get the progress of the generation.</param>
        /// <returns>A list of <see cref="RequestOutput"/> objects containing the generated completions in the same order as the input prompt ids.</returns>
        public RequestOutput[] Generate(IList<IList<int>> promptTokenIds, ISamplingMethod? samplingMethod = null, 
            IStoppingCriteria? stoppingCriteria = null, ProgressCallback? progressCallback = null)
        {
            if(promptTokenIds.Count == 0)
            {
                return [];
            }
            samplingMethod ??= GlobalConfig.DefaultSamplingMethod;
            stoppingCriteria ??= GlobalConfig.DefaultStoppingCriteria;

            // Add requests to the engine.
            foreach(var prompt in promptTokenIds)
            {
                AddRequest(null, samplingMethod, stoppingCriteria);
            }
            return RunEngine(progressCallback);
        }

        private void AddRequest(string? prompt, ISamplingMethod samplingMethod, IStoppingCriteria stoppingCriteria, IList<int>? promptTokenIds = null)
        {
            var requestId = _counter.Next().ToString();
            _engine.AddRequest(requestId, prompt, samplingMethod, stoppingCriteria, promptTokenIds, DateTime.Now);
        }

        private RequestOutput[] RunEngine(ProgressCallback? callback)
        {
            float numRequests = _engine.NumUnfinishedRequests;
            Debug.Assert(numRequests - 0 > 0.0001f); // assert the number of requests is not zero
            int completedRequests = 0;
            List<RequestOutput> outputs = new();
            while (_engine.HasUnfinishedRequests)
            {
                var stepOutputs = _engine.Step();
                foreach(var output in stepOutputs)
                {
                    if (output.Finished)
                    {
                        outputs.Add(output);
                        if(callback is not null)
                        {
                            completedRequests++;
                            callback(completedRequests / numRequests);
                        }
                    }
                }
            }
            // Sort the outputs by request ID.
            // This is necessary because some requests may be finished earlier than its previous requests.
            return outputs.OrderBy(o => o.RequestId).ToArray();
        }


        /// <summary>
        /// A callback function to used for reporting the progress of the generation.
        /// It will be called every time a new request is completed.
        /// </summary>
        /// <param name="progress">The progress in percentage.</param>
        public delegate void ProgressCallback(float progress);
    }

}

