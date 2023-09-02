using System;
using System.Collections.Generic;

#pragma warning disable
// ReSharper disable all

namespace LLama.OldVersion
{
    public enum ChatRole
    {
        Human,
        Assistant
    }

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record EmbeddingUsage(int PromptTokens, int TotalTokens);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record EmbeddingData(int Index, string Object, float[] Embedding);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record Embedding(string Object, string Model, EmbeddingData[] Data, EmbeddingUsage Usage);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record CompletionLogprobs(int[] TextOffset, float[] TokenLogProbs, string[] Tokens, Dictionary<string, float>[] TopLogprobs);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record CompletionChoice(string Text, int Index, CompletionLogprobs? Logprobs, string? FinishReason);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record CompletionUsage(int PromptTokens, int CompletionTokens, int TotalTokens);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record CompletionChunk(string Id, string Object, int Created, string Model, CompletionChoice[] Choices);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record Completion(string Id, string Object, int Created, string Model, CompletionChoice[] Choices, CompletionUsage Usage);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record ChatCompletionMessage(ChatRole Role, string Content, string? Name = null);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record ChatCompletionChoice(int Index, ChatCompletionMessage Message, string? FinishReason);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record ChatCompletion(string Id, string Object, int Created, string Model, ChatCompletionChoice[] Choices, CompletionUsage Usage);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record ChatCompletionChunkDelta(string? Role, string? Content);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record ChatCompletionChunkChoice(int Index, ChatCompletionChunkDelta Delta, string? FinishReason);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record ChatCompletionChunk(string Id, string Model, string Object, int Created, ChatCompletionChunkChoice[] Choices);

    [Obsolete("The entire LLama.OldVersion namespace will be removed")]
    public record ChatMessageRecord(ChatCompletionMessage Message, DateTime Time);
}
