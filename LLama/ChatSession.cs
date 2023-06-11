using LLama.Common;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace LLama
{
    public class ChatSession
    {
        private readonly string defaultUserName = "User";
        private readonly string defaultAssistantName = "Assistant";
        private readonly string defaultSystemName = "System";
        private readonly string defaultUnknownName = "??";
        private ILLamaExecutor _executor;
        private ChatHistory _history;
        public ILLamaExecutor Executor => _executor;
        public ChatHistory History => _history;
        public SessionParams Params { get; set; }

        public ChatSession(ILLamaExecutor executor, SessionParams? sessionParams = null)
        {
            _executor = executor;
            _history = new ChatHistory();
            Params = sessionParams ?? new SessionParams();
        }

        public virtual string BuildTextFromHistory(ChatHistory history)
        {
            StringBuilder sb = new();
            var userName = Params.UserName ?? defaultUserName;
            var assistantName = Params.AssistantName ?? defaultAssistantName;
            var systemName = Params.SystemName ?? defaultSystemName;
            foreach (var message in history.Messages)
            {
                if (message.AuthorRole == AuthorRole.User)
                {
                    sb.AppendLine($"{userName}: {message.Content}");
                }
                else if (message.AuthorRole == AuthorRole.System)
                {
                    sb.AppendLine($"{systemName}: {message.Content}");
                }
                else if (message.AuthorRole == AuthorRole.Unknown)
                {
                    sb.AppendLine($"{defaultUnknownName}: {message.Content}");
                }
                else if (message.AuthorRole == AuthorRole.Assistant)
                {
                    sb.AppendLine($"{assistantName}: {message.Content}");
                }
            }
            return sb.ToString();
        }

        public virtual string CropNameFromText(string text, AuthorRole role)
        {
            if (!string.IsNullOrEmpty(Params.UserName) && role == AuthorRole.User && text.StartsWith($"{Params.UserName}:"))
            {
                text = text.Substring($"{Params.UserName}:".Length).TrimStart();
            }
            else if (!string.IsNullOrEmpty(Params.AssistantName) && role == AuthorRole.Assistant && text.EndsWith($"{Params.AssistantName}:"))
            {
                text = text.Substring(0, text.Length - $"{Params.AssistantName}:".Length).TrimEnd();
            }
            if (_executor is InstructExecutor && role == AuthorRole.Assistant && text.EndsWith("\n> "))
            {
                text = text.Substring(0, text.Length - "\n> ".Length).TrimEnd();
            }
            return text;
        }

        /// <summary>
        /// Get the response from the LLama model with chat histories.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="inferenceParams"></param>
        /// <returns></returns>
        public IEnumerable<string> Chat(ChatHistory history, InferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            var prompt = BuildTextFromHistory(history);
            History.AddMessage(AuthorRole.User, prompt);
            StringBuilder sb = new();
            foreach (var result in _executor.Infer(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.AddMessage(AuthorRole.Assistant, CropNameFromText(sb.ToString(), AuthorRole.Assistant));
        }

        /// <summary>
        /// Get the response from the LLama model. Note that prompt could not only be the preset words, 
        /// but also the question you want to ask.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="inferenceParams"></param>
        /// <returns></returns>
        public IEnumerable<string> Chat(string prompt, InferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            History.AddMessage(AuthorRole.User, prompt);
            StringBuilder sb = new();
            foreach (var result in _executor.Infer(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.AddMessage(AuthorRole.Assistant, CropNameFromText(sb.ToString(), AuthorRole.Assistant));
        }

        /// <summary>
        /// Get the response from the LLama model with chat histories.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="inferenceParams"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> ChatAsync(ChatHistory history, InferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var prompt = BuildTextFromHistory(history);
            History.AddMessage(AuthorRole.User, prompt);
            StringBuilder sb = new();
            await foreach (var result in _executor.InferAsync(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.AddMessage(AuthorRole.Assistant, CropNameFromText(sb.ToString(), AuthorRole.Assistant));
        }

        public async IAsyncEnumerable<string> ChatAsync(string prompt, InferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            History.AddMessage(AuthorRole.User, prompt);
            StringBuilder sb = new();
            await foreach (var result in _executor.InferAsync(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.AddMessage(AuthorRole.Assistant, CropNameFromText(sb.ToString(), AuthorRole.Assistant));
        }
    }



}