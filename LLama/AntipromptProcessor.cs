using System;
using System.Collections.Generic;

namespace LLama
{
    /// <summary>
    /// AntipromptProcessor keeps track of past tokens looking for any set Anti-Prompts
    /// </summary>
    public sealed class AntipromptProcessor
    {
        private int _longestAntiprompt;
        private readonly List<string> _antiprompts = new();

        private string _buffer = string.Empty;


        /// <summary>
        /// Initializes a new instance of the <see cref="AntipromptProcessor"/> class.
        /// </summary>
        /// <param name="antiprompts">The antiprompts.</param>
        public AntipromptProcessor(IEnumerable<string>? antiprompts = null)
        {
            if (antiprompts != null)
                SetAntiprompts(antiprompts);
        }

        /// <summary>
        /// Add an antiprompt to the collection
        /// </summary>
        /// <param name="antiprompt"></param>
        public void AddAntiprompt(string antiprompt)
        {
            _antiprompts.Add(antiprompt);
            _longestAntiprompt = Math.Max(_longestAntiprompt, antiprompt.Length);
        }

        /// <summary>
        /// Overwrite all current antiprompts with a new set
        /// </summary>
        /// <param name="antiprompts"></param>
        public void SetAntiprompts(IEnumerable<string> antiprompts)
        {
            _antiprompts.Clear();
            _antiprompts.AddRange(antiprompts);

            _longestAntiprompt = 0;
            foreach (var antiprompt in _antiprompts)
                _longestAntiprompt = Math.Max(_longestAntiprompt, antiprompt.Length);

            _buffer = string.Empty;
        }

        /// <summary>
        /// Add some text and check if the buffer now ends with any antiprompt
        /// </summary>
        /// <param name="text"></param>
        /// <returns>true if the text buffer ends with any antiprompt</returns>
        public bool Add(string text)
        {
            _buffer += text;

            // When the string gets very long (4x antiprompt length) trim it down (to 2x antiprompt length).
            // This trimming leaves a lot of extra characters because two sequences can be considered "equal" in unicode
            // even with different numbers of characters. Hopefully there are enough characters here to handle all those weird circumstances!
            var maxLength = Math.Max(32, _longestAntiprompt * 4);
            var trimLength = Math.Max(16, _longestAntiprompt * 2);
            if (_buffer.Length > maxLength)
                _buffer = _buffer.Substring(_buffer.Length - trimLength);

            foreach (var antiprompt in _antiprompts)
                if (_buffer.EndsWith(antiprompt, StringComparison.CurrentCulture))
                    return true;

            return false;
        }
    }
}
