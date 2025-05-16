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
        private static readonly ChatClientMetadata s_metadata = new(nameof(LLamaExecutorChatClient));
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
        public void Dispose() { }

        /// <inheritdoc/>
        public object? GetService(Type serviceType, object? serviceKey = null) =>
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? s_metadata :
            serviceType?.IsInstanceOfType(_executor) is true ? _executor :
            serviceType?.IsInstanceOfType(this) is true ? this :
            null;

        /// <inheritdoc/>
        public async Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            var result = _executor.InferAsync(CreatePrompt(messages), CreateInferenceParams(options), cancellationToken);

            StringBuilder text = new();
            await foreach (var token in _outputTransform.TransformAsync(result))
            {
                text.Append(token);
            }

            var message = new ChatMessage(ChatRole.Assistant, text.ToString())
            {
                MessageId = Guid.NewGuid().ToString("N"),
            };

            return new(message)
            {
                CreatedAt = DateTime.UtcNow,
            };
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages, ChatOptions? options = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var result = _executor.InferAsync(CreatePrompt(messages), CreateInferenceParams(options), cancellationToken);

            string messageId = Guid.NewGuid().ToString("N");
            await foreach (var token in _outputTransform.TransformAsync(result))
            {
                yield return new(ChatRole.Assistant, token) 
                {
                    CreatedAt = DateTime.UtcNow,
                    MessageId = messageId,
                };
            }
        }

        /// <summary>Format the chat messages into a string prompt.</summary>
        private string CreatePrompt(IEnumerable<ChatMessage> messages)
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
        private InferenceParams CreateInferenceParams(ChatOptions? options)
        {
            InferenceParams ip = options?.RawRepresentationFactory?.Invoke(this) as InferenceParams ?? new();

            ip.AntiPrompts = [.. s_antiPrompts, .. ip.AntiPrompts];
            ip.MaxTokens = options?.MaxOutputTokens ?? 256; // arbitrary upper limit
            ip.SamplingPipeline = new DefaultSamplingPipeline()
            {
                FrequencyPenalty = options?.FrequencyPenalty ?? (ip.SamplingPipeline as DefaultSamplingPipeline)?.FrequencyPenalty ?? s_defaultPipeline.FrequencyPenalty,
                PresencePenalty = options?.PresencePenalty ?? (ip.SamplingPipeline as DefaultSamplingPipeline)?.PresencePenalty ?? s_defaultPipeline.PresencePenalty,
                PreventEOS = (ip.SamplingPipeline as DefaultSamplingPipeline)?.PreventEOS ?? s_defaultPipeline.PreventEOS,
                PenalizeNewline = (ip.SamplingPipeline as DefaultSamplingPipeline)?.PenalizeNewline ?? s_defaultPipeline.PenalizeNewline,
                RepeatPenalty = (ip.SamplingPipeline as DefaultSamplingPipeline)?.RepeatPenalty ?? s_defaultPipeline.RepeatPenalty,
                PenaltyCount = (ip.SamplingPipeline as DefaultSamplingPipeline)?.PenaltyCount ?? s_defaultPipeline.PenaltyCount,
                Grammar = (ip.SamplingPipeline as DefaultSamplingPipeline)?.Grammar ?? s_defaultPipeline.Grammar,
                GrammarOptimization = (ip.SamplingPipeline as DefaultSamplingPipeline)?.GrammarOptimization ?? s_defaultPipeline.GrammarOptimization,
                LogitBias = (ip.SamplingPipeline as DefaultSamplingPipeline)?.LogitBias ?? s_defaultPipeline.LogitBias,
                MinKeep = (ip.SamplingPipeline as DefaultSamplingPipeline)?.MinKeep ?? s_defaultPipeline.MinKeep,
                MinP = (ip.SamplingPipeline as DefaultSamplingPipeline)?.MinP ?? s_defaultPipeline.MinP,
                Seed = options?.Seed is long seed ? (uint)seed : (uint)(t_random ??= new()).Next(),
                Temperature = options?.Temperature ?? s_defaultPipeline.Temperature,
                TopP = options?.TopP ?? s_defaultPipeline.TopP,
                TopK = options?.TopK ?? s_defaultPipeline.TopK,
                TypicalP = (ip.SamplingPipeline as DefaultSamplingPipeline)?.TypicalP ?? s_defaultPipeline.TypicalP,
            };

            return ip;
        }

        /// <summary>A default transform that appends "Assistant: " to the end.</summary>
        private sealed class AppendAssistantHistoryTransform : LLamaTransforms.DefaultHistoryTransform
        {
            public override string HistoryToText(ChatHistory history) => 
                $"{base.HistoryToText(history)}{AuthorRole.Assistant}: ";
        }
    }
}