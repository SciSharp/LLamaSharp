using LLama.Abstractions;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LLama.InteractiveExecutor;

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
            if (Executor is StatelessExecutor)
            {

            }
            else if (Executor is StatefulExecutorBase statefulExecutor)
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
        /// Generates a response for a given user prompt and manages history state for the user.
        /// This will always pass the whole history to the model. Don't pass a whole history
        /// to this method as the user prompt will be appended to the history of the current session.
        /// If more control is needed, use the other overload of this method that accepts a ChatHistory object.
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="inferenceParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns generated text of the assistant message.</returns>
        public async IAsyncEnumerable<string> ChatAsync(string prompt, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var inputTransform in InputTransformPipeline)
                prompt = inputTransform.Transform(prompt);

            History.Messages.Add(new ChatHistory.Message(AuthorRole.User, prompt));

            if (_executor is InteractiveExecutor executor)
            {
                InteractiveExecutorState state = (InteractiveExecutorState)executor.GetStateData();
                prompt = state.IsPromptRun
                    ? HistoryTransform.HistoryToText(History)
                    : prompt;
            }

            StringBuilder sb = new();

            await foreach (var result in ChatAsyncInternal(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
                sb.Append(result);
            }

            string assistantMessage = sb.ToString();

            // Remove end tokens from the assistant message
            // if defined in inferenceParams.AntiPrompts.
            // We only want the response that was generated and not tokens
            // that are delimiting the beginning or end of the response.
            if (inferenceParams?.AntiPrompts != null)
            {
                foreach (var stopToken in inferenceParams.AntiPrompts)
                {
                    assistantMessage = assistantMessage.Replace(stopToken, "");
                }
            }

            History.Messages.Add(new ChatHistory.Message(AuthorRole.Assistant, assistantMessage));
        }

        /// <summary>
        /// Generates a response for a given chat history. This method does not manage history state for the user.
        /// If you want to e.g. truncate the history of a session to fit into the model's context window,
        /// use this method and pass the truncated history to it. If you don't need this control, use the other
        /// overload of this method that accepts a user prompt instead.
        /// </summary>
        /// <param name="history"></param>
        /// <param name="inferenceParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns generated text of the assistant message.</returns>
        public async IAsyncEnumerable<string> ChatAsync(ChatHistory history, IInferenceParams? inferenceParams = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (history.Messages.Count == 0)
            {
                throw new ArgumentException("History must contain at least one message.");
            }

            string prompt;
            if (_executor is InteractiveExecutor executor)
            {
                InteractiveExecutorState state = (InteractiveExecutorState)executor.GetStateData();

                prompt = state.IsPromptRun
                    ? HistoryTransform.HistoryToText(History)
                    : history.Messages.Last().Content;
            }
            else
            {
                prompt = history.Messages.Last().Content;
            }

            await foreach (var result in ChatAsyncInternal(prompt, inferenceParams, cancellationToken))
            {
                yield return result;
            }
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