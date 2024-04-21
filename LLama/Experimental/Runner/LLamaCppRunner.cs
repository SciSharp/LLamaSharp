using LLama.Abstractions;
using LLama.Experimental.Abstractions;
using LLama.Experimental.Common;
using LLama.Experimental.Runner.LLamaCpp;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Runner
{
    /// <summary>
    /// Using llama.cpp backend to execute the model.
    /// </summary>
    public sealed class LLamaCppRunner: ModelRunnerBase, IModelRunner
    {
        public LLamaWeights ModelWeights { get; }

        public LLamaContext Context { get; }

        public LLamaCppRunner(LLamaWeights modelWeights, IContextParams contextParams)
        {
            ModelWeights = modelWeights;
            Context = new LLamaContext(modelWeights, contextParams);
        }

        /// <inheritdoc/>
        public SamplerOutput ExecuteModel(IEnumerable<SequenceGroupMetadata> seqGroupMetadataList)
        {
            var modelInput = PrepareInputs(seqGroupMetadataList);
            var samplingMetadata = PrepareSample(seqGroupMetadataList, modelInput.PromptLengths, modelInput.SubqueryLengths);
            var llamaCppRunnerInput = new LLamaCppRunnerInput(modelInput);
            var nativeBatch = llamaCppRunnerInput.ToLLamaNativeBatch(out var pinHolder);

            // TODO: is global lock still necessary?

            // Batched inference
            Context.Decode(nativeBatch);

            // Get the logits
            Dictionary<int, LogitsGenerator> seqIdToLogits = new();
            for(int i = 0; i < llamaCppRunnerInput.WithLogits.Length; i++)
            {
                if (llamaCppRunnerInput.WithLogits[i])
                {
                    for(int j = 0; j < llamaCppRunnerInput.SeqIds[i].Length; j++)
                    {
                        if (seqIdToLogits.ContainsKey(llamaCppRunnerInput.SeqIds[i][j]))
                        {
                            throw new Exception("Duplicate sequence id found when getting logits.");
                        }
                        else
                        {
                            seqIdToLogits.Add(llamaCppRunnerInput.SeqIds[i][j], new LogitsGenerator(i, Context));
                        }
                    }
                }
            }

            // Sample the logits to get output tokens.
            List<SequenceGroupOutput> outputs = new();
            foreach(var seqGroupMetadata in seqGroupMetadataList)
            {
                List<SequenceOutput> sequenceOutputs = new();
                foreach(var seqId in seqGroupMetadata.SeqData.Keys)
                {
                    var output = seqGroupMetadata.SamplingMethod.SampleSequence(seqIdToLogits[seqId].GetLogits(), seqId, samplingMetadata);
                    sequenceOutputs.Add(output);
                }
                outputs.Add(new SequenceGroupOutput(sequenceOutputs));
            }

            return new SamplerOutput(outputs);
        }
        
        public void Dispose()
        {
            // It should dispose context but not model weight.
            Context.Dispose();
        }
    }
}
