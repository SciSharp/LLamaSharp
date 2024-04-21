using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// Status of a sequence.
    /// </summary>
    public enum SequenceStatus
    {
        /// <summary>
        /// The sequence is waiting for scheduling.
        /// </summary>
        Waiting, 

        /// <summary>
        /// The sequence is running.
        /// </summary>
        Running, 

        /// <summary>
        /// The sequence has been swapped out due to some reasons.
        /// </summary>
        Swapped, 

        /// <summary>
        /// The sequence has been finished because it's stopped by a stopping criteria.
        /// </summary>
        FinishStopped, 

        /// <summary>
        /// The sequence has been finished because it reaches the maximum length.
        /// </summary>
        FinishLengthCapped, 

        /// <summary>
        /// The sequence has been finished because it's aborted.
        /// </summary>
        FinishAborted, 

        /// <summary>
        /// The sequence will never be processed for some reasons. Please check if the prompt length is too long.
        /// </summary>
        FinishIgnored
    }

    /// <inheritdoc/>
    public static class SequenceStatusExtensions
    {
        /// <summary>
        ///  Get the finished reason in OpenAI style
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetFinishedReason(this SequenceStatus status)
        {
            return status switch
            {
                SequenceStatus.FinishStopped => "stop",
                SequenceStatus.FinishLengthCapped => "length",
                SequenceStatus.FinishAborted => "abort",
                SequenceStatus.FinishIgnored => "length",
                _ => ""
            };
        }
    }
}
