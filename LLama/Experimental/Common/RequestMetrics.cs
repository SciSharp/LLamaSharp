using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Experimental.Common
{
    /// <summary>
    /// Metrics associated with a request.
    /// </summary>
    public class RequestMetrics
    {
        /// <summary>
        /// The time when the request arrived.
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// The time when the request was first scheduled.
        /// </summary>
        public DateTime? FirstScheduledTime { get; set; }

        /// <summary>
        /// The time when the first token was generated.
        /// </summary>
        public DateTime? FirstTokenTime { get; set; }

        /// <summary>
        /// The time when the last token was generated.
        /// </summary>
        public DateTime? LastTokenTime { get; set; }

        /// <summary>
        /// The time the request spent in the queue.
        /// </summary>
        public TimeSpan? TimeInQueue { get; set; }

        /// <summary>
        /// The time when the request was finished.
        /// </summary>
        public DateTime? FinishedTime { get; set; }
    }
}
