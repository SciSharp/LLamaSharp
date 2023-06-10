using System;
using System.Collections.Generic;
using System.Text;

namespace LLama.Old
{
    public enum ChatRole
    {
        Human,
        Assistant
    }
    public record EmbeddingUsage(int PromptTokens, int TotalTokens);

    public record EmbeddingData(int Index, string Object, float[] Embedding);

    public record Embedding(string Object, string Model, EmbeddingData[] Data, EmbeddingUsage Usage);

    public record CompletionLogprobs(int[] TextOffset, float[] TokenLogProbs, string[] Tokens, Dictionary<string, float>[] TopLogprobs);

    public record CompletionChoice(string Text, int Index, CompletionLogprobs? Logprobs, string? FinishReason);

    public record CompletionUsage(int PromptTokens, int CompletionTokens, int TotalTokens);

    public record CompletionChunk(string Id, string Object, int Created, string Model, CompletionChoice[] Choices);

    public record Completion(string Id, string Object, int Created, string Model, CompletionChoice[] Choices, CompletionUsage Usage);

    public record ChatCompletionMessage(ChatRole Role, string Content, string? Name = null);

    public record ChatCompletionChoice(int Index, ChatCompletionMessage Message, string? FinishReason);

    public record ChatCompletion(string Id, string Object, int Created, string Model, ChatCompletionChoice[] Choices, CompletionUsage Usage);

    public record ChatCompletionChunkDelta(string? Role, string? Content);

    public record ChatCompletionChunkChoice(int Index, ChatCompletionChunkDelta Delta, string? FinishReason);

    public record ChatCompletionChunk(string Id, string Model, string Object, int Created, ChatCompletionChunkChoice[] Choices);

    public record ChatMessageRecord(ChatCompletionMessage Message, DateTime Time);
}
