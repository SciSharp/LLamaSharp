using LLama.Native;
using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Batched
{
    /// <summary>
    /// Stores the data, status, and other information of a sequence when inference the LLM.
    /// </summary>
    internal class Sequence
    {
        private LLamaPos _end;
        private bool _forked;

        /// <summary>
        /// Whether the sequence is a forked one.
        /// </summary>
        public bool Forked
        {
            get => _forked;
            internal set => _forked = value;
        }

        /// <summary>
        /// Unique ID for this conversation
        /// </summary>
        public LLamaSeqId Id { get; }

        /// <summary>
        /// Total number of tokens in this conversation, cannot exceed the context length.
        /// </summary>
        public LLamaPos TokenCount
        {
            get => _end.Value;
            internal set => _end = value;
        }

        internal Sequence(LLamaSeqId id, bool forked)
        {
            Id = id;
            _forked = forked;
        }

        internal Sequence(LLamaSeqId id, bool forked, LLamaPos tokensCount)
        {
            Id = id;
            _forked = forked;
            _end = tokensCount;
        }
    }
}
