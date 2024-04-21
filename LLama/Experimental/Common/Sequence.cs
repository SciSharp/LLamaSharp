using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// Stores the data, status, and other information of a sequence.
    /// </summary>
    public sealed class Sequence
    {
        /// <summary>
        /// The ID of the sequence.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The prompt of the sequence.
        /// </summary>
        public string? Prompt { get; }

        /// <summary>
        /// Data used for computation of this sequence.
        /// </summary>
        public SequenceData Data { get; private set; }

        /// <summary>
        /// The output text of the sequence. 
        /// Note that it should only be set when you want to implement some interfaces yourself.
        /// </summary>
        public string OutputText { get; internal set; }

        /// <summary>
        /// Input + output token IDs. 
        /// Note that it should only be set when you want to implement some interfaces yourself.
        /// </summary>
        public IEnumerable<int> TokenIds => Data. TokenIds;

        /// <summary>
        /// Length of the sequence data.
        /// </summary>
        public int Length => Data.Length;

        /// <summary>
        /// The status of the sequence.
        /// </summary>
        public SequenceStatus Status { get; internal set; }

        /// <summary>
        /// The stopping string of the sequence if it stops because of this string.
        /// </summary>
        public string? StoppingString { get; internal set; }

        /// <summary>
        /// The stopping token of the sequence if it stops because of this token.
        /// </summary>
        public int? StoppingTokenId { get; internal set; }

        /// <summary>
        /// The offset of the sequence in the decoding process. 
        /// It's useful when the tokenizer may use more than 1 token id to represent a token.
        /// </summary>
        public int IncrementalDecodingOffset { get; internal set; }

        /// <summary>
        /// Whether the sequence has finished.
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return Status is SequenceStatus.FinishStopped 
                    or SequenceStatus.FinishLengthCapped 
                    or SequenceStatus.FinishAborted 
                    or SequenceStatus.FinishIgnored;
            }
        }

        /// <summary>
        /// The output token ids of the sequence.
        /// </summary>
        public IList<int> OutputTokens => Data.OutputTokenIds;

        /// <summary>
        /// Whether the sequence is at prefill stage.
        /// </summary>
        public bool IsPrefill => Data.Stage == SequenceStage.Prefill;

        /// <summary>
        /// Get the number of new tokens to be computed.
        /// </summary>
        public int NumNewTokens
        {
            get
            {
                if (Data.Stage == SequenceStage.Decode)
                {
                    return 1;
                }
                return Data.NumUncomputedTokens;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prompt"></param>
        /// <param name="promptTokens"></param>
        public Sequence(int id, string? prompt, IList<int> promptTokens)
        {
            Id = id;
            Prompt = prompt;
            Data = new SequenceData(promptTokens);
            OutputText = "";
            Status = SequenceStatus.Waiting;
            IncrementalDecodingOffset = Data.PromptTokenIds.Count;

            // TODO: deal with incremental detokenization.
        }

        /// <summary>
        /// Add a token id to the output ids of the sequence data.
        /// </summary>
        /// <param name="tokenId"></param>
        public void AppendToken(int tokenId)
            // TODO: logprobs?
        {
            Data.AppendToken(tokenId);
        }

        /// <summary>
        /// Get a new sequence with same data but new id.
        /// </summary>
        /// <param name="newSeqId"></param>
        /// <returns></returns>
        public Sequence Fork(int newSeqId)
        {
            // clone the current data.
            var clone = (Sequence)MemberwiseClone();
            clone.Data = new SequenceData(
                new List<int>(Data.PromptTokenIds),
                new List<int>(Data.OutputTokenIds)
            );
            // set new id
            clone.Id = newSeqId;
            return clone;
        }
    }
}
