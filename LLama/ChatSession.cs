using LLama.Abstractions;
using LLama.Common;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LLama
{
    /// <summary>
    /// The main chat session class.
    /// </summary>
    public class ChatSession
    {
        private readonly ILLamaExecutor _executor;
        private readonly ChatHistory _history;

        private const string _executorStateFilename = "ExecutorState.json";
        private const string _modelStateFilename = "ModelState.st";

        /// <summary>
        /// The executor for this session.
        /// </summary>
        public ILLamaExecutor Executor => _executor;
        /// <summary>
        /// The chat history for this session.
        /// </summary>
        public ChatHistory History => _history;
        /// <summary>
        /// The history transform used in this session.
        /// </summary>
        public IHistoryTransform HistoryTransform { get; set; } = new LLamaTransforms.DefaultHistoryTransform();
        /// <summary>
        /// The input transform pipeline used in this session.
        /// </summary>
        public List<ITextTransform> InputTransformPipeline { get; set; } = new();
        /// <summary>
        /// The output transform used in this session.
        /// </summary>
        public ITextStreamTransform OutputTransform = new LLamaTransforms.EmptyTextOutputStreamTransform();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executor">The executor for this session</param>
        public ChatSession(ILLamaExecutor executor)
        {
            _executor = executor;
            _history = new ChatHistory();
        }

        /// <summary>
        /// Use a custom history transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public ChatSession WithHistoryTransform(IHistoryTransform transform)
        {
            HistoryTransform = transform;
            return this;
        }

        /// <summary>
        /// Add a text transform to the input transform pipeline.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public ChatSession AddInputTransform(ITextTransform transform)
        {
            InputTransformPipeline.Add(transform);
            return this;
        }

        /// <summary>
        /// Use a custom output transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public ChatSession WithOutputTransform(ITextStreamTransform transform)
        {
            OutputTransform = transform;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The directory name to save the session. If the directory does not exist, a new directory will be created.</param>
        public virtual void SaveSession(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            _executor.Context.SaveState(Path.Combine(path, _modelStateFilename));
            if(Executor is StatelessExecutor)
            {

            }
            else if(Executor is StatefulExecutorBase statefulExecutor)
            {
                statefulExecutor.SaveState(Path.Combine(path, _executorStateFilename));
            }
            else
            {
                throw new System.NotImplementedException("You're using a customized executor. Please inherit ChatSession and rewrite the method.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The directory name to load the session.</param>
        public virtual void LoadSession(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException($"Directory {path} does not exist.");
            }
            _executor.Context.LoadState(Path.Combine(path, _modelStateFilename));
            if (Executor is StatelessExecutor)
            {

            }
            else if (Executor is StatefulExecutorBase statefulExecutor)
            {
                statefulExecutor.LoadState(Path.Combine(path, _executorStateFilename));
            }
            else
            {
                throw new System.NotImplementedException("You're using a customized executor. Please inherit ChatSession and rewrite the method.");
            }
        }

        /// <summary>
        /// Get the response from the LLama model with chat histories.
        /// </summary>
        /// <param name="history"></param>
        /// <param name="inferenceParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IEnumerable<string> Chat(ChatHistory history, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public IEnumerable<string> Chat(string prompt, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
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
        /// <param name="history"></param>
        /// <param name="inferenceParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> ChatAsync(ChatHistory history, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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

        /// <summary>
        /// Get the response from the LLama model with chat histories asynchronously.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="inferenceParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<string> ChatAsync(string prompt, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
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

        private IEnumerable<string> ChatInternal(string prompt, IInferenceParams? inferenceParams = null, CancellationToken cancellationToken = default)
        {
            var results = _executor.Infer(prompt, inferenceParams, cancellationToken);
            return OutputTransform.Transform(results);
        }

        private async IAsyncEnumerable<string> ChatAsyncInternal(string prompt, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var results = _executor.InferAsync(prompt, inferenceParams, cancellationToken);
            await foreach (var item in OutputTransform.TransformAsync(results).WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}