using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Common
{
    public class SessionParams
    {
        public string? UserName { get; set; }
        public string? AssistantName { get; set; }
        public string? SystemName { get; set; }
        /// <summary>
        /// The prefix of input text. Note that this only works when you 
        /// use the API with text as input.
        /// </summary>
        public string? InputPrefix { get; set; }
        /// <summary>
        /// The suffix of input text. Note that this only works when you 
        /// use the API with text as input.
        /// </summary>
        public string? InputSuffix { get; set; }
        /// <summary>
        /// Whether to trim the names from the text output at the start and end.
        /// </summary>
        public bool TrimNamesFromOutput { get; set; } = false;
    }
}
