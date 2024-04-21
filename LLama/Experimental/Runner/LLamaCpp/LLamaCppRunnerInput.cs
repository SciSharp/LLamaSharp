using LLama.Experimental.Common;
using LLama.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace LLama.Experimental.Runner.LLamaCpp
{
    /// <summary>
    /// Input special for <see cref="LLamaCppRunner"/>.
    /// </summary>
    public class LLamaCppRunnerInput
        // TODO: get this from a pool?
    {
        public int[] TokenIds { get; }

        public int[] Positions { get; }

        public int[] SeqIdCount { get; }

        public int[][] SeqIds { get; }

        public bool[] WithLogits { get; }

        public IntPtr[] SeqIdsPtrs { get; }

        /// <summary>
        /// Construct from <see cref="ModelRunnerInput"/>.
        /// </summary>
        /// <param name="input"></param>
        public LLamaCppRunnerInput(ModelRunnerInput input)
        {
            Debug.Assert(input.TokenIds.Length == input.Positions.Length);
            Debug.Assert(input.TokenIds.Length == input.SeqIds.Length);
            Debug.Assert(input.TokenIds.Length == input.WithLogits.Length);
            TokenIds = input.TokenIds;
            Positions = input.Positions;

            // TODO: Now we never put a token in multiple sequences,
            // which may impact on the speed of the model in some cases.
            // We should consider to support this in the future.
            SeqIdCount = Enumerable.Repeat(1, TokenIds.Length).ToArray();
            SeqIds = new int[TokenIds.Length][];
            for(int i = 0; i < input.SeqIds.Length; i++)
            {
                SeqIds[i] = [input.SeqIds[i]];
            }
            WithLogits = input.WithLogits;
            SeqIdsPtrs = new IntPtr[SeqIds.Length];
        }

        /// <summary>
        /// Convert <see cref="LLamaCppRunnerInput"/> to <see cref="LLamaNativeBatch"/>.
        /// 
        /// [WARNING] You must hold the pin holder until the returned value will no longer be used.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pinHolder"></param>
        /// <returns></returns>
        internal LLamaNativeBatch ToLLamaNativeBatch(out GroupDisposable pinHolder)
        {
            pinHolder = new GroupDisposable();

            unsafe
            {
                var batch = new LLamaNativeBatch
                {
                    n_tokens = TokenIds.Length,
                    logits = (byte*)pinHolder.Add(WithLogits.AsMemory().Pin()).Pointer,

                    n_seq_id = (int*)pinHolder.Add(SeqIdCount.AsMemory().Pin()).Pointer,
                    pos = (LLamaPos*)pinHolder.Add(Positions.AsMemory().Pin()).Pointer,
                    seq_id = (LLamaSeqId**)pinHolder.Add(SeqIdsPtrs.AsMemory().Pin()).Pointer,

                    // embd is not currently supported, so this is always null!
                    embd = null,

                    // Note that if embd is **not null** then this will be null!
                    tokens = (LLamaToken*)pinHolder.Add(TokenIds.AsMemory().Pin()).Pointer,
                };

                // Create pointers to each of the arrays in turns
                for (var i = 0; i < SeqIdsPtrs.Length; i++)
                    SeqIdsPtrs[i] = (IntPtr)pinHolder.Add(SeqIds[i].AsMemory().Pin()).Pointer;

                return batch;
            }
        }
    }
}
