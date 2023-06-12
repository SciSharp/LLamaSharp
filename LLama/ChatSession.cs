using LLama.Common;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace LLama
{
    public class ChatSession
    {
        private ILLamaExecutor _executor;
        private ChatHistory _history;
        public ILLamaExecutor Executor => _executor;
        public ChatHistory History => _history;
        public SessionParams Params { get; set; }
        public IHistoryTransform HistoryTransform { get; set; } = new LLamaTransforms.DefaultHistoryTransform();
        public List<ITextTransform> InputTransformPipeline { get; set; } = new();
        public ITextStreamTransform OutputTransform = new LLamaTransforms.EmptyTextOutputStreamTransform();

        public ChatSession(ILLamaExecutor executor, SessionParams? sessionParams = null)
        {
            _executor = executor;
            _history = new ChatHistory();
            Params = sessionParams ?? new SessionParams();
        }

        public ChatSession WithHistoryTransform(IHistoryTransform transform)
        {
            HistoryTransform = transform;
            return this;
        }

        public ChatSession AddInputTransform(ITextTransform transform)
        {
            InputTransformPipeline.Add(transform);
            return this;
        }

        public ChatSession WithOutputTransform(ITextStreamTransform transform)
        {
            OutputTransform = transform;
            return this;
        }

        /// <summary>
        /// Get the response from the LLama model with chat histories.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="inferenceParams"></param>
        /// <returns></returns>
        public IEnumerable<string> Chat(ChatHistory history, InferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            var prompt = HistoryTransform.HistoryToText(history);
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.User, prompt).Messages);
            StringBuilder sb = new();
            foreach (var result in ChatInternal(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.Assistant, sb.ToString()).Messages);
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
            foreach(var inputTransform in InputTransformPipeline)
            {
                prompt = inputTransform.Transform(prompt);
            }
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.User, prompt).Messages);
            StringBuilder sb = new();
            foreach (var result in ChatInternal(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.Assistant, sb.ToString()).Messages);
        }

        /// <summary>
        /// Get the response from the LLama model with chat histories.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="inferenceParams"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> ChatAsync(ChatHistory history, InferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var prompt = HistoryTransform.HistoryToText(history);
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.User, prompt).Messages);
            StringBuilder sb = new();
            await foreach (var result in ChatAsyncInternal(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.Assistant, sb.ToString()).Messages);
        }

        public async IAsyncEnumerable<string> ChatAsync(string prompt, InferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var inputTransform in InputTransformPipeline)
            {
                prompt = inputTransform.Transform(prompt);
            }
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.User, prompt).Messages);
            StringBuilder sb = new();
            await foreach (var result in ChatAsyncInternal(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }
            History.Messages.AddRange(HistoryTransform.TextToHistory(AuthorRole.Assistant, sb.ToString()).Messages);
        }

        private IEnumerable<string> ChatInternal(string prompt, InferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            var results = _executor.Infer(prompt, inferenceParams, cancellationToken);
            return OutputTransform.Transform(results);
        }

        private async IAsyncEnumerable<string> ChatAsyncInternal(string prompt, InferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var results = _executor.InferAsync(prompt, inferenceParams, cancellationToken);
            await foreach (var item in OutputTransform.TransformAsync(results))
            {
                yield return item;
            }
        }
    }
}