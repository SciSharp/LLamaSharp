using LLama.Abstractions;
using LLama.Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace LLama
{
    public class LLamaTransforms
    {
        public class DefaultHistoryTransform : IHistoryTransform
        {
            private readonly string defaultUserName = "User";
            private readonly string defaultAssistantName = "Assistant";
            private readonly string defaultSystemName = "System";
            private readonly string defaultUnknownName = "??";

            string _userName;
            string _assistantName;
            string _systemName;
            string _unknownName;
            bool _isInstructMode;
            public DefaultHistoryTransform(string? userName = null, string? assistantName = null, 
                string? systemName = null, string? unknownName = null, bool isInstructMode = false)
            {
                _userName = userName ?? defaultUserName;
                _assistantName = assistantName ?? defaultAssistantName;
                _systemName = systemName ?? defaultSystemName;
                _unknownName = unknownName ?? defaultUnknownName;
                _isInstructMode = isInstructMode;
            }

            public virtual string HistoryToText(ChatHistory history)
            {
                StringBuilder sb = new();
                foreach (var message in history.Messages)
                {
                    if (message.AuthorRole == AuthorRole.User)
                    {
                        sb.AppendLine($"{_userName}: {message.Content}");
                    }
                    else if (message.AuthorRole == AuthorRole.System)
                    {
                        sb.AppendLine($"{_systemName}: {message.Content}");
                    }
                    else if (message.AuthorRole == AuthorRole.Unknown)
                    {
                        sb.AppendLine($"{_unknownName}: {message.Content}");
                    }
                    else if (message.AuthorRole == AuthorRole.Assistant)
                    {
                        sb.AppendLine($"{_assistantName}: {message.Content}");
                    }
                }
                return sb.ToString();
            }

            public virtual ChatHistory TextToHistory(AuthorRole role, string text)
            {
                ChatHistory history = new ChatHistory();
                history.AddMessage(role, TrimNamesFromText(text, role));
                return history;
            }

            public virtual string TrimNamesFromText(string text, AuthorRole role)
            {
                if (role == AuthorRole.User && text.StartsWith($"{_userName}:"))
                {
                    text = text.Substring($"{_userName}:".Length).TrimStart();
                }
                else if (role == AuthorRole.Assistant && text.EndsWith($"{_assistantName}:"))
                {
                    text = text.Substring(0, text.Length - $"{_assistantName}:".Length).TrimEnd();
                }
                if (_isInstructMode && role == AuthorRole.Assistant && text.EndsWith("\n> "))
                {
                    text = text.Substring(0, text.Length - "\n> ".Length).TrimEnd();
                }
                return text;
            }
        }

        /// <summary>
        /// A text input transform that only trims the text.
        /// </summary>
        public class NaiveTextInputTransform : ITextTransform
        {
            public NaiveTextInputTransform()
            {
                
            }

            public string Transform(string text)
            {
                return text.Trim();
            }
        }

        public class EmptyTextOutputStreamTransform : ITextStreamTransform
        {
            public IEnumerable<string> Transform(IEnumerable<string> tokens)
            {
                return tokens;
            }

            public IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<string> tokens)
            {
                return tokens;
            }
        }

        public class KeywordTextOutputStreamTransform : ITextStreamTransform
        {
            HashSet<string> _keywords;
            int _maxKeywordLength;
            bool _removeAllMatchedTokens;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="keywords">Keywords that you want to remove from the response.</param>
            /// <param name="redundancyLength">The extra length when searching for the keyword. For example, if your only keyword is "highlight", 
            /// maybe the token you get is "\r\nhighligt". In this condition, if redundancyLength=0, the token cannot be successfully matched because the length of "\r\nhighligt" (10)
            /// has already exceeded the maximum length of the keywords (8). On the contrary, setting redundancyLengyh >= 2 leads to successful match.
            /// The larger the redundancyLength is, the lower the processing speed. But as an experience, it won't introduce too much performance impact when redundancyLength <= 5 </param>
            /// <param name="removeAllMatchedTokens">If set to true, when getting a matched keyword, all the related tokens will be removed. Otherwise only the part of keyword will be removed.</param>
            public KeywordTextOutputStreamTransform(IEnumerable<string> keywords, int redundancyLength = 3, bool removeAllMatchedTokens = false)
            {
                _keywords = new(keywords);
                _maxKeywordLength = keywords.Select(x => x.Length).Max() + redundancyLength;
                _removeAllMatchedTokens = removeAllMatchedTokens;
            }

            public IEnumerable<string> Transform(IEnumerable<string> tokens)
            {
                var window = new Queue<string>();

                foreach (var s in tokens)
                {
                    window.Enqueue(s);
                    var current = string.Join("", window);
                    if (_keywords.Any(x => current.Contains(x)))
                    {
                        int total = window.Count;
                        for (int i = 0; i < total; i++)
                        {
                            window.Dequeue();
                        }
                    }
                    if(current.Length >= _maxKeywordLength)
                    {
                        int total = window.Count;
                        for (int i = 0; i < total; i++)
                        {
                            yield return window.Dequeue();
                        }
                    }
                }
                int totalCount = window.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    yield return window.Dequeue();
                }
            }

            public async IAsyncEnumerable<string> TransformAsync(IAsyncEnumerable<string> tokens)
            {
                var window = new Queue<string>();

                await foreach (var s in tokens)
                {
                    window.Enqueue(s);
                    var current = string.Join("", window);
                    if (_keywords.Any(x => current.Contains(x)))
                    {
                        var matchedKeyword = _keywords.First(x => current.Contains(x));
                        int total = window.Count;
                        for (int i = 0; i < total; i++)
                        {
                            window.Dequeue();
                        }
                        if (!_removeAllMatchedTokens)
                        {
                            yield return current.Replace(matchedKeyword, "");
                        }
                    }
                    if (current.Length >= _maxKeywordLength)
                    {
                        int total = window.Count;
                        for (int i = 0; i < total; i++)
                        {
                            yield return window.Dequeue();
                        }
                    }
                }
                int totalCount = window.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    yield return window.Dequeue();
                }
            }
        }
    }
}
