using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LLama.Common;
using LLama.Sampling;
using Microsoft.Extensions.AI;

namespace LLama.Abstractions;

/// <summary>
/// Extension methods to the <see cref="LLamaExecutorExtensions" /> interface.
/// </summary>
public static class LLamaExecutorExtensions
{
    /// <summary>Gets an <see cref="IChatClient"/> instance for the specified <see cref="ILLamaExecutor"/>.</summary>
    /// <param name="executor">The executor.</param>
    /// <param name="historyTransform">The <see cref="IHistoryTransform"/> to use to transform an input list messages into a prompt.</param>
    /// <param name="outputTransform">The <see cref="ITextStreamTransform"/> to use to transform the output into text.</param>
    /// <returns>An <see cref="IChatClient"/> instance for the provided <see cref="ILLamaExecutor" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="executor"/> is null.</exception>
    public static IChatClient AsChatClient(
        this ILLamaExecutor executor,
        IHistoryTransform? historyTransform = null,
        ITextStreamTransform? outputTransform = null) =>
        new LLamaExecutorChatClient(
            executor ?? throw new ArgumentNullException(nameof(executor)),
            historyTransform, 
            outputTransform);

    private sealed class LLamaExecutorChatClient(
        ILLamaExecutor executor,
        IHistoryTransform? historyTransform = null,
        ITextStreamTransform? outputTransform = null) : IChatClient
    {
        private static readonly InferenceParams s_defaultParams = new();
        private static readonly DefaultSamplingPipeline s_defaultPipeline = new();
        private static readonly string[] s_antiPrompts = ["User:", "Assistant:", "System:"];
        [ThreadStatic]
        private static Random? t_random;

        private readonly ILLamaExecutor _executor = executor;
        private readonly IHistoryTransform _historyTransform = historyTransform ?? new AppendAssistantHistoryTransform();
        private readonly ITextStreamTransform _outputTransform = outputTransform ??
            new LLamaTransforms.KeywordTextOutputStreamTransform(s_antiPrompts);

        /// <inheritdoc/>
        public ChatClientMetadata Metadata { get; } = new(nameof(LLamaExecutorChatClient));

        /// <inheritdoc/>
        public void Dispose() { }

        /// <inheritdoc/>
        public TService? GetService<TService>(object? key = null) where TService : class =>
            typeof(TService) == typeof(ILLamaExecutor) ? (TService)_executor :
            this as TService;

        /// <inheritdoc/>
        public async Task<ChatCompletion> CompleteAsync(
            IList<ChatMessage> chatMessages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var result = _executor.InferAsync(CreatePrompt(chatMessages), CreateInferenceParams(options), cancellationToken);

            StringBuilder text = new();
            await foreach (var token in _outputTransform.TransformAsync(result))
            {
                text.Append(token);
            }

            return new(new ChatMessage(ChatRole.Assistant, text.ToString()))
            {
                CreatedAt = DateTime.UtcNow,
            };
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(
            IList<ChatMessage> chatMessages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var result = _executor.InferAsync(CreatePrompt(chatMessages), CreateInferenceParams(options), cancellationToken);

            await foreach (var token in _outputTransform.TransformAsync(result))
            {
                yield return new() 
                {
                    CreatedAt = DateTime.UtcNow,
                    Role = ChatRole.Assistant,
                    Text = token,
                };
            }
        }

        /// <summary>Format the chat messages into a string prompt.</summary>
        private string CreatePrompt(IList<ChatMessage> messages)
        {
            if (messages is null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            ChatHistory history = new();

            if (_executor is not StatefulExecutorBase seb ||
                seb.GetStateData() is InteractiveExecutor.InteractiveExecutorState { IsPromptRun: true })
            {
                foreach (var message in messages)
                {
                    history.AddMessage(
                        message.Role == ChatRole.System ? AuthorRole.System :
                        message.Role == ChatRole.Assistant ? AuthorRole.Assistant :
                        AuthorRole.User,
                        string.Concat(message.Contents.OfType<TextContent>()));
                }
            }
            else
            {
                // Stateless executor with IsPromptRun = false: use only the last message.
                history.AddMessage(AuthorRole.User, string.Concat(messages.LastOrDefault()?.Contents.OfType<TextContent>() ?? []));
            }
         
            return _historyTransform.HistoryToText(history);
        }

        /// <summary>Convert the chat options to inference parameters.</summary>
        private static InferenceParams? CreateInferenceParams(ChatOptions? options)
        {
            List<string> antiPrompts = new(s_antiPrompts);
            if (options?.AdditionalProperties?.TryGetValue(nameof(InferenceParams.AntiPrompts), out IReadOnlyList<string>? anti) is true)
            {
                antiPrompts.AddRange(anti);
            }

            return new()
            {
                AntiPrompts = antiPrompts,
                TokensKeep = options?.AdditionalProperties?.TryGetValue(nameof(InferenceParams.TokensKeep), out int tk) is true ? tk : s_defaultParams.TokensKeep,
                MaxTokens = options?.MaxOutputTokens ?? 256, // arbitrary upper limit
                SamplingPipeline = new DefaultSamplingPipeline()
                {
                    AlphaFrequency = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.AlphaFrequency), out float af) is true ? af : s_defaultPipeline.AlphaFrequency,
                    AlphaPresence = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.AlphaPresence), out float ap) is true ? ap : s_defaultPipeline.AlphaPresence,
                    PenalizeEOS = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.PenalizeEOS), out bool eos) is true ? eos : s_defaultPipeline.PenalizeEOS,
                    PenalizeNewline = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.PenalizeNewline), out bool pnl) is true ? pnl : s_defaultPipeline.PenalizeNewline,
                    RepeatPenalty = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.RepeatPenalty), out float rp) is true ? rp : s_defaultPipeline.RepeatPenalty,
                    RepeatPenaltyCount = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.RepeatPenaltyCount), out int rpc) is true ? rpc : s_defaultPipeline.RepeatPenaltyCount,
                    Grammar = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.Grammar), out Grammar? g) is true ? g : s_defaultPipeline.Grammar,
                    MinKeep = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.MinKeep), out int mk) is true ? mk : s_defaultPipeline.MinKeep,
                    MinP = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.MinP), out float mp) is true ? mp : s_defaultPipeline.MinP,
                    Seed = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.Seed), out uint seed) is true ? seed : (uint)(t_random ??= new()).Next(),
                    TailFreeZ = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.TailFreeZ), out float tfz) is true ? tfz : s_defaultPipeline.TailFreeZ,
                    Temperature = options?.Temperature ?? 0,
                    TopP = options?.TopP ?? 0,
                    TopK = options?.TopK ?? s_defaultPipeline.TopK,
                    TypicalP = options?.AdditionalProperties?.TryGetValue(nameof(DefaultSamplingPipeline.TypicalP), out float tp) is true ? tp : s_defaultPipeline.TypicalP,
                },
            };
        }

        /// <summary>A default transform that appends "Assistant: " to the end.</summary>
        private sealed class AppendAssistantHistoryTransform : LLamaTransforms.DefaultHistoryTransform
        {
            public override string HistoryToText(ChatHistory history) => 
                $"{base.HistoryToText(history)}{AuthorRole.Assistant}: ";
        }
    }
}