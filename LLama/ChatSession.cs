using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LLama.Abstractions;
using LLama.Common;
using static LLama.InteractiveExecutor;
using static LLama.LLamaContext;
using static LLama.StatefulExecutorBase;

namespace LLama;

/// <summary>
/// The main chat session class.
/// </summary>
public class ChatSession
{
    /// <summary>
    /// The filename for the serialized model state (KV cache, etc).
    /// </summary>
    public const string MODEL_STATE_FILENAME = "ModelState.st";
    /// <summary>
    /// The filename for the serialized executor state.
    /// </summary>
    public const string EXECUTOR_STATE_FILENAME = "ExecutorState.json";
    /// <summary>
    /// The filename for the serialized chat history.
    /// </summary>
    public const string HISTORY_STATE_FILENAME = "ChatHistory.json";
    /// <summary>
    /// The filename for the serialized input transform pipeline.
    /// </summary>
    public const string INPUT_TRANSFORM_FILENAME = "InputTransform.json";
    /// <summary>
    /// The filename for the serialized output transform.
    /// </summary>
    public const string OUTPUT_TRANSFORM_FILENAME = "OutputTransform.json";
    /// <summary>
    /// The filename for the serialized history transform.
    /// </summary>
    public const string HISTORY_TRANSFORM_FILENAME = "HistoryTransform.json";

    /// <summary>
    /// The executor for this session.
    /// </summary>
    public ILLamaExecutor Executor { get; private set; }

    /// <summary>
    /// The chat history for this session.
    /// </summary>
    public ChatHistory History { get; private set; } = new();

    /// <summary>
    /// The history transform used in this session.
    /// </summary>
    public IHistoryTransform HistoryTransform { get; set; } = new LLamaTransforms.DefaultHistoryTransform();

    /// <summary>
    /// The input transform pipeline used in this session.
    /// </summary>
    public List<ITextTransform> InputTransformPipeline { get; set; } = [];

    /// <summary>
    /// The output transform used in this session.
    /// </summary>
    public ITextStreamTransform OutputTransform = new LLamaTransforms.EmptyTextOutputStreamTransform();

    /// <summary>
    /// Create a new chat session and preprocess history.
    /// </summary>
    /// <param name="executor">The executor for this session</param>
    /// <param name="history">History for this session</param>
    /// <param name="transform">History Transform for this session</param>
    /// <returns>A new chat session.</returns>
    public static async Task<ChatSession> InitializeSessionFromHistoryAsync(
        ILLamaExecutor executor, ChatHistory history, IHistoryTransform? transform = null)
    {
        if (executor is not StatefulExecutorBase statefulExecutor)
        {
            throw new ArgumentException("Executor must have a StatefulExecutorBase", nameof(executor));
        }
        var session = new ChatSession(executor, history);
        if (transform != null)
        {
            session = session.WithHistoryTransform(transform);
        }

        await statefulExecutor.PrefillPromptAsync(session.HistoryTransform.HistoryToText(history));
        return session;
    }

    /// <summary>
    /// Create a new chat session.
    /// </summary>
    /// <param name="executor">The executor for this session</param>
    public ChatSession(ILLamaExecutor executor)
    {
        // Check if executor has StatefulExecutorBase as base class
        if (executor is not StatefulExecutorBase)
        {
            throw new ArgumentException("Executor must have a StatefulExecutorBase", nameof(executor));
        }

        Executor = executor;
    }

    /// <summary>
    /// Create a new chat session with a custom history.
    /// </summary>
    /// <param name="executor"></param>
    /// <param name="history"></param>
    public ChatSession(ILLamaExecutor executor, ChatHistory history)
        : this(executor)
    {
        History = history;
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
    /// Save a session from a directory.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public void SaveSession(string path)
    {
        GetSessionState().Save(path);
    }

    /// <summary>
    /// Get the session state.
    /// </summary>
    /// <returns>SessionState object representing session state in-memory</returns>
    public SessionState GetSessionState()
    {
        var executorState = ((StatefulExecutorBase)Executor).GetStateData();
        return new SessionState(
            executorState.PastTokensCount > 0
            ? Executor.Context.GetState() : null,
            executorState,
            History,
            InputTransformPipeline,
            OutputTransform,
            HistoryTransform);
    }

    /// <summary>
    /// Load a session from a session state.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="loadTransforms">If true loads transforms saved in the session state.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public void LoadSession(SessionState state, bool loadTransforms = true)
    {
        if (Executor is StatefulExecutorBase statefulExecutor)
        {
            if (state.ExecutorState is not null)
            {
                statefulExecutor.LoadState(state.ExecutorState);
            }
        }
        if (state.ContextState is null)
        {
            Executor.Context.NativeHandle.MemoryClear();
        }
        else
        {
            Executor.Context.LoadState(state.ContextState);
        }
        History = new ChatHistory(state.History);
        if (loadTransforms)
        {
            InputTransformPipeline = state.InputTransformPipeline.Select(t => t.Clone()).ToList();
            OutputTransform = state.OutputTransform.Clone();
            HistoryTransform = state.HistoryTransform.Clone();
        }
    }

    /// <summary>
    /// Load a session from a directory.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="loadTransforms">If true loads transforms saved in the session state.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public void LoadSession(string path, bool loadTransforms = true)
    {
        var state = SessionState.Load(path);
        // Handle non-polymorphic serialization of executor state
        if (state.ExecutorState is null)
        {
            var executorPath = Path.Combine(path, EXECUTOR_STATE_FILENAME);
            ((StatefulExecutorBase)Executor).LoadState(filename: executorPath);
        }
        LoadSession(state, loadTransforms);
    }

    /// <summary>
    /// Add a message to the chat history.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public ChatSession AddMessage(ChatHistory.Message message)
    {
        // If current message is a system message, only allow the history to be empty
        if (message.AuthorRole == AuthorRole.System && History.Messages.Count > 0)
        {
            throw new ArgumentException("Cannot add a system message after another message", nameof(message));
        }

        // If current message is a user message, only allow the history to be empty,
        // or the previous message to be a system message or assistant message.
        if (message.AuthorRole == AuthorRole.User)
        {
            ChatHistory.Message? lastMessage = History.Messages.LastOrDefault();
            if (lastMessage is not null && lastMessage.AuthorRole == AuthorRole.User)
            {
                throw new ArgumentException("Cannot add a user message after another user message", nameof(message));
            }
        }

        // If the current message is an assistant message,
        // the previous message must be a user message.
        if (message.AuthorRole == AuthorRole.Assistant)
        {
            ChatHistory.Message? lastMessage = History.Messages.LastOrDefault();
            if (lastMessage is null
                || lastMessage.AuthorRole != AuthorRole.User)
            {
                throw new ArgumentException("Assistant message must be preceded with a user message", nameof(message));
            }
        }

        History.AddMessage(message.AuthorRole, message.Content);
        return this;
    }

    /// <summary>
    /// Add a system message to the chat history.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public ChatSession AddSystemMessage(string content)
        => AddMessage(new ChatHistory.Message(AuthorRole.System, content));

    /// <summary>
    /// Add an assistant message to the chat history.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public ChatSession AddAssistantMessage(string content)
        => AddMessage(new ChatHistory.Message(AuthorRole.Assistant, content));

    /// <summary>
    /// Add a user message to the chat history.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public ChatSession AddUserMessage(string content)
        => AddMessage(new ChatHistory.Message(AuthorRole.User, content));

    /// <summary>
    /// Remove the last message from the chat history.
    /// </summary>
    /// <returns></returns>
    public ChatSession RemoveLastMessage()
    {
        History.Messages.RemoveAt(History.Messages.Count - 1);
        return this;
    }

    /// <summary>
    /// Compute KV cache for the message and add it to the chat history.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<ChatSession> AddAndProcessMessage(ChatHistory.Message message)
    {
        if (Executor is not StatefulExecutorBase statefulExecutor)
        {
            throw new InvalidOperationException("Executor must be a StatefulExecutorBase to support pre-processing of system messages.");
        }
        AddMessage(message);
        var content = message.Content;
        if (message.AuthorRole != AuthorRole.Assistant)
        {
            foreach (var inputTransform in InputTransformPipeline)
            {
                content = inputTransform.Transform(content);
            }
        }

        await statefulExecutor.PrefillPromptAsync(content);
        return this;
    }

    /// <summary>
    /// Compute KV cache for the system message and add it to the chat history.
    /// </summary>
    public Task<ChatSession> AddAndProcessSystemMessage(string content)
        => AddAndProcessMessage(new ChatHistory.Message(AuthorRole.System, content));

    /// <summary>
    /// Compute KV cache for the user message and add it to the chat history.
    /// </summary>
    public Task<ChatSession> AddAndProcessUserMessage(string content)
        => AddAndProcessMessage(new ChatHistory.Message(AuthorRole.User, content));

    /// <summary>
    /// Compute KV cache for the assistant message and add it to the chat history.
    /// </summary>
    public Task<ChatSession> AddAndProcessAssistantMessage(string content)
        => AddAndProcessMessage(new ChatHistory.Message(AuthorRole.Assistant, content));

    /// <summary>
    /// Replace a user message with a new message and remove all messages after the new message.
    /// This is useful when the user wants to edit a message. And regenerate the response.
    /// </summary>
    /// <param name="oldMessage"></param>
    /// <param name="newMessage"></param>
    /// <returns></returns>
    public ChatSession ReplaceUserMessage(
        ChatHistory.Message oldMessage,
        ChatHistory.Message newMessage)
    {
        if (oldMessage.AuthorRole != AuthorRole.User)
        {
            throw new ArgumentException("Old message must be a user message", nameof(oldMessage));
        }

        if (newMessage.AuthorRole != AuthorRole.User)
        {
            throw new ArgumentException("New message must be a user message", nameof(newMessage));
        }

        int index = History.Messages.IndexOf(oldMessage);
        if (index == -1)
        {
            throw new ArgumentException("Old message does not exist in history", nameof(oldMessage));
        }

        History.Messages[index] = newMessage;

        // Remove all message after the new message
        History.Messages.RemoveRange(index + 1, History.Messages.Count - index - 1);

        return this;
    }

    /// <summary>
    /// Chat with the model.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inferenceParams"></param>
    /// <param name="applyInputTransformPipeline"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async IAsyncEnumerable<string> ChatAsync(
        ChatHistory.Message message,
        bool applyInputTransformPipeline,
        IInferenceParams? inferenceParams = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // The message must be a user message
        if (message.AuthorRole != AuthorRole.User)
        {
            throw new ArgumentException("Message must be a user message", nameof(message));
        }

        // Apply input transform pipeline
        if (applyInputTransformPipeline)
        {
            foreach (var inputTransform in InputTransformPipeline)
            {
                message.Content = inputTransform.Transform(message.Content);
            }
        }

        // Add the user's message to the history
        AddUserMessage(message.Content);

        // Prepare prompt variable
        string prompt;

        // Check if the session history was restored from a previous session
        // or added as part of new chat session history.
        InteractiveExecutorState state = (InteractiveExecutorState)((StatefulExecutorBase)Executor).GetStateData();

        // If "IsPromptRun" is true, the session was newly started.
        if (state.IsPromptRun)
        {
            // If the session history was added as part of new chat session history,
            // convert the complete history including system message and manually added history
            // to a prompt that adhere to the prompt template specified in the HistoryTransform class implementation.
            prompt = HistoryTransform.HistoryToText(History);
        }
        else
        {
            // If the session was restored from a previous session,
            // convert only the current message to the prompt with the prompt template
            // specified in the HistoryTransform class implementation that is provided.
            ChatHistory singleMessageHistory = HistoryTransform.TextToHistory(message.AuthorRole, message.Content);
            prompt = HistoryTransform.HistoryToText(singleMessageHistory);
        }

        StringBuilder assistantMessage = new();

        try
        {
            await foreach (var textToken in ChatAsyncInternal(prompt, inferenceParams, cancellationToken))
            {
                assistantMessage.Append(textToken);
                yield return textToken;
            }
        }
        finally
        {
            // Add the assistant message to the history
            AddAssistantMessage(assistantMessage.ToString());
        }
    }

    /// <summary>
    /// Chat with the model.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inferenceParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public IAsyncEnumerable<string> ChatAsync(
        ChatHistory.Message message,
        IInferenceParams? inferenceParams = null,
        CancellationToken cancellationToken = default)
    {
        return ChatAsync(
            message,
            applyInputTransformPipeline: true,
            inferenceParams,
            cancellationToken);
    }

    /// <summary>
    /// Chat with the model.
    /// </summary>
    /// <param name="history"></param>
    /// <param name="applyInputTransformPipeline"></param>
    /// <param name="inferenceParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public IAsyncEnumerable<string> ChatAsync(
        ChatHistory history,
        bool applyInputTransformPipeline,
        IInferenceParams? inferenceParams = null,
        CancellationToken cancellationToken = default)
    {
        ChatHistory.Message lastMessage = history.Messages.LastOrDefault()
            ?? throw new ArgumentException("History must contain at least one message", nameof(history));

        foreach (
            ChatHistory.Message message
            in history.Messages.Take(history.Messages.Count - 1))
        {
            // Apply input transform pipeline
            if (applyInputTransformPipeline
                && message.AuthorRole == AuthorRole.User)
            {
                foreach (
                    var inputTransform
                    in InputTransformPipeline)
                {
                    message.Content = inputTransform.Transform(message.Content);
                }
            }

            AddMessage(message);
        }

        return ChatAsync(
            lastMessage,
            applyInputTransformPipeline,
            inferenceParams,
            cancellationToken);
    }

    /// <summary>
    /// Chat with the model.
    /// </summary>
    /// <param name="history"></param>
    /// <param name="inferenceParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public IAsyncEnumerable<string> ChatAsync(
        ChatHistory history,
        IInferenceParams? inferenceParams = null,
        CancellationToken cancellationToken = default)
    {
        return ChatAsync(
            history,
            applyInputTransformPipeline: true,
            inferenceParams,
            cancellationToken);
    }

    /// <summary>
    /// Regenerate the last assistant message.
    /// </summary>
    /// <param name="inferenceParams"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async IAsyncEnumerable<string> RegenerateAssistantMessageAsync(
        InferenceParams? inferenceParams = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Make sure the last message is an assistant message (response from the LLM).
        ChatHistory.Message? lastAssistantMessage = History.Messages.LastOrDefault();

        if (lastAssistantMessage is null
            || lastAssistantMessage.AuthorRole != AuthorRole.Assistant)
        {
            throw new InvalidOperationException("Last message must be an assistant message");
        }

        // Remove the last assistant message from the history.
        RemoveLastMessage();

        // Get the last user message.
        ChatHistory.Message? lastUserMessage = History.Messages.LastOrDefault();

        if (lastUserMessage is null
            || lastUserMessage.AuthorRole != AuthorRole.User)
        {
            throw new InvalidOperationException("Last message must be a user message");
        }

        // Remove the last user message from the history.
        RemoveLastMessage();

        // Regenerate the assistant message.
        await foreach (
            string textToken
            in ChatAsync(
                lastUserMessage,
                applyInputTransformPipeline: false,
                inferenceParams,
                cancellationToken))
        {
            yield return textToken;
        }
    }

    private async IAsyncEnumerable<string> ChatAsyncInternal(
        string prompt,
        IInferenceParams? inferenceParams = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var results = Executor.InferAsync(prompt, inferenceParams, cancellationToken);

        await foreach (
            string textToken
            in OutputTransform
                .TransformAsync(results)
                .WithCancellation(cancellationToken))
        {
            yield return textToken;
        }
    }
}

/// <summary>
/// The state of a chat session in-memory.
/// </summary>
public record SessionState
{
    /// <summary>
    /// Saved executor state for the session in JSON format.
    /// </summary>
    public ExecutorBaseState? ExecutorState { get; set; }

    /// <summary>
    /// Saved context state (KV cache) for the session.
    /// </summary>
    public State? ContextState { get; set; }

    /// <summary>
    /// The input transform pipeline used in this session.
    /// </summary>
    public ITextTransform[] InputTransformPipeline { get; set; } = [];

    /// <summary>
    /// The output transform used in this session.
    /// </summary>
    public ITextStreamTransform OutputTransform { get; set; } = new LLamaTransforms.EmptyTextOutputStreamTransform();

    /// <summary>
    /// The history transform used in this session.
    /// </summary>
    public IHistoryTransform HistoryTransform { get; set; } = new LLamaTransforms.DefaultHistoryTransform();

    /// <summary>
    /// The chat history messages for this session.
    /// </summary>
    public ChatHistory.Message[] History { get; set; } = [];

    /// <summary>
    /// Create a new session state.
    /// </summary>
    /// <param name="contextState"></param>
    /// <param name="executorState"></param>
    /// <param name="history"></param>
    /// <param name="inputTransformPipeline"></param>
    /// <param name="outputTransform"></param>
    /// <param name="historyTransform"></param>
    public SessionState(
        State? contextState, ExecutorBaseState executorState,
        ChatHistory history, List<ITextTransform> inputTransformPipeline,
        ITextStreamTransform outputTransform, IHistoryTransform historyTransform)
    {
        ContextState = contextState;
        ExecutorState = executorState;
        History = history.Messages.ToArray();
        InputTransformPipeline = inputTransformPipeline.Select(t => t.Clone()).ToArray();
        OutputTransform = outputTransform.Clone();
        HistoryTransform = historyTransform.Clone();
    }

    /// <summary>
    /// Save the session state to folder.
    /// </summary>
    /// <param name="path"></param>
    public void Save(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or whitespace", nameof(path));
        }

        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }

        Directory.CreateDirectory(path);

        string modelStateFilePath = Path.Combine(path, ChatSession.MODEL_STATE_FILENAME);
        if (ContextState != null)
            using (var stateStream = File.Create(modelStateFilePath))
                ContextState?.Save(stateStream);

        string executorStateFilepath = Path.Combine(path, ChatSession.EXECUTOR_STATE_FILENAME);
        File.WriteAllText(executorStateFilepath, JsonSerializer.Serialize(ExecutorState));

        string historyFilepath = Path.Combine(path, ChatSession.HISTORY_STATE_FILENAME);
        File.WriteAllText(historyFilepath, new ChatHistory(History).ToJson());

        string inputTransformFilepath = Path.Combine(path, ChatSession.INPUT_TRANSFORM_FILENAME);
        File.WriteAllText(inputTransformFilepath, JsonSerializer.Serialize(InputTransformPipeline));

        string outputTransformFilepath = Path.Combine(path, ChatSession.OUTPUT_TRANSFORM_FILENAME);
        File.WriteAllText(outputTransformFilepath, JsonSerializer.Serialize(OutputTransform));

        string historyTransformFilepath = Path.Combine(path, ChatSession.HISTORY_TRANSFORM_FILENAME);
        File.WriteAllText(historyTransformFilepath, JsonSerializer.Serialize(HistoryTransform));
    }

    /// <summary>
    /// Load the session state from folder.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Throws when session state is incorrect</exception>
    public static SessionState Load(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or whitespace", nameof(path));
        }

        if (!Directory.Exists(path))
        {
            throw new ArgumentException("Directory does not exist", nameof(path));
        }

        var modelStateFilePath = Path.Combine(path, ChatSession.MODEL_STATE_FILENAME);
        State? contextState = default;
        if (File.Exists(modelStateFilePath))
            using (var modelStateStream = File.OpenRead(modelStateFilePath))
                contextState = State.Load(modelStateStream);

        string executorStateFilepath = Path.Combine(path, ChatSession.EXECUTOR_STATE_FILENAME);
        var executorState = JsonSerializer.Deserialize<ExecutorBaseState>(File.ReadAllText(executorStateFilepath));

        string historyFilepath = Path.Combine(path, ChatSession.HISTORY_STATE_FILENAME);
        string historyJson = File.ReadAllText(historyFilepath);
        var history = ChatHistory.FromJson(historyJson)
            ?? throw new ArgumentException("History file is invalid", nameof(path));

        string inputTransformFilepath = Path.Combine(path, ChatSession.INPUT_TRANSFORM_FILENAME);
        ITextTransform[] inputTransforms;
        try
        {
            inputTransforms = File.Exists(inputTransformFilepath) ?
                (JsonSerializer.Deserialize<ITextTransform[]>(File.ReadAllText(inputTransformFilepath))
                ?? throw new ArgumentException("Input transform file is invalid", nameof(path)))
                : [];
        }
        catch (JsonException)
        {
            throw new ArgumentException("Input transform file is invalid", nameof(path));
        }

        string outputTransformFilepath = Path.Combine(path, ChatSession.OUTPUT_TRANSFORM_FILENAME);

        ITextStreamTransform outputTransform;
        try
        {
            outputTransform = File.Exists(outputTransformFilepath) ?
            (JsonSerializer.Deserialize<ITextStreamTransform>(File.ReadAllText(outputTransformFilepath))
                       ?? throw new ArgumentException("Output transform file is invalid", nameof(path)))
            : new LLamaTransforms.EmptyTextOutputStreamTransform();
        }
        catch (JsonException)
        {
            throw new ArgumentException("Output transform file is invalid", nameof(path));
        }

        string historyTransformFilepath = Path.Combine(path, ChatSession.HISTORY_TRANSFORM_FILENAME);
        IHistoryTransform historyTransform;
        try
        {
            historyTransform = File.Exists(historyTransformFilepath) ?
                (JsonSerializer.Deserialize<IHistoryTransform>(File.ReadAllText(historyTransformFilepath))
                           ?? throw new ArgumentException("History transform file is invalid", nameof(path)))
                : new LLamaTransforms.DefaultHistoryTransform();
        }
        catch (JsonException)
        {
            throw new ArgumentException("History transform file is invalid", nameof(path));
        }

        return new SessionState(
            contextState,
            executorState!,
            history,
            inputTransforms.ToList(),
            outputTransform,
            historyTransform);
    }
}
