#nullable enable

using System.Text;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;

namespace LLama.Unittest;

/// <summary>
/// Optional end-to-end regression for the PLaMo-3 tokenizer and translation model.
/// </summary>
public class PlamoCompatibilityTests
{
    /// <summary>
    /// Whether the caller supplied a local PLaMo-3 Translate Q4_K_M model.
    /// </summary>
    public static bool IsModelAvailable => File.Exists(Environment.GetEnvironmentVariable("LLAMASHARP_PLAMO_MODEL_PATH"));

    /// <summary>
    /// Exercise tokenization, decoding, sampling and state restoration with a real PLaMo-3 model.
    /// </summary>
    [Fact(Skip = "Set LLAMASHARP_PLAMO_MODEL_PATH to the PLaMo-3 Translate Q4_K_M GGUF.", SkipUnless = nameof(IsModelAvailable))]
    public async Task TranslatesJapaneseAndRestoresState()
    {
        var parameters = new ModelParams(Environment.GetEnvironmentVariable("LLAMASHARP_PLAMO_MODEL_PATH")!)
        {
            ContextSize = 512,
            BatchSize = 128,
            GpuLayerCount = 0,
            LoadMode = LLamaLoadMode.MemoryMap,
        };
        using var weights = LLamaWeights.LoadFromFile(parameters);
        Assert.Equal(LLamaVocabType.PLAMO3, weights.NativeHandle.Vocab.Type);
        using var context = weights.CreateContext(parameters);
        const string text = "会議は来週の月曜日に変更されました。";
        var tokens = context.Tokenize(text, addBos: false);
        var decoder = new StreamingTokenDecoder(context);
        decoder.AddRange(tokens);
        Assert.Equal(text, decoder.Read());

        const string prompt = "<|plamo:tag|>dataset<|plamo:msg|>translation"
            + "<|plamo:tag|>input<|plamo:msg|>" + text
            + "<|plamo:tag|>output<|plamo:key|>lang<|plamo:val|>en<|plamo:msg|>";
        var batch = new LLamaBatch();
        var promptTokens = context.Tokenize(prompt, special: true);
        for (var i = 0; i < promptTokens.Length; i++)
        {
            batch.Add(promptTokens[i], i, (LLamaSeqId)0, i == promptTokens.Length - 1);
        }

        var cancellation = TestContext.Current.CancellationToken;
        Assert.Equal(DecodeResult.Ok, await context.DecodeAsync(batch, cancellation));
        var savedPosition = context.NativeHandle.MemorySequenceMaxPosition(LLamaSeqId.Zero);
        using var state = context.GetState();
        using var pipeline = new DefaultSamplingPipeline { Temperature = 0, RepeatPenalty = 1 };
        var first = pipeline.Sample(context.NativeHandle, promptTokens.Length - 1);

        batch.Clear();
        batch.Add(first, promptTokens.Length, LLamaSeqId.Zero, true);
        Assert.Equal(DecodeResult.Ok, await context.DecodeAsync(batch, cancellation));
        Assert.Equal((LLamaPos)promptTokens.Length, context.NativeHandle.MemorySequenceMaxPosition(LLamaSeqId.Zero));
        var second = pipeline.Sample(context.NativeHandle, 0);

        context.LoadState(state);
        Assert.Equal(savedPosition, context.NativeHandle.MemorySequenceMaxPosition(LLamaSeqId.Zero));
        pipeline.Reset();
        Assert.Equal(DecodeResult.Ok, await context.DecodeAsync(batch, cancellation));
        Assert.Equal(second, pipeline.Sample(context.NativeHandle, 0));

        var result = new StringBuilder();
        var executor = new StatelessExecutor(weights, parameters);
        using var translationPipeline = new DefaultSamplingPipeline { Temperature = 0, RepeatPenalty = 1 };
        var inference = new InferenceParams
        {
            MaxTokens = 64,
            AntiPrompts = ["<|plamo:tag|>", "<|plamo:bos|>"],
            SamplingPipeline = translationPipeline,
        };
        await foreach (var part in executor.InferAsync(prompt, inference, cancellation))
        {
            result.Append(part);
        }

        TestContext.Current.TestOutputHelper!.WriteLine(result.ToString());
        Assert.Contains("Monday", result.ToString(), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("meeting", result.ToString(), StringComparison.OrdinalIgnoreCase);
        Assert.True(context.NativeHandle.GetTimings().PromptEval >= TimeSpan.Zero);

        using var dry = SafeLLamaSamplerChainHandle.Create(LLamaSamplerChainParams.Default());
        dry.AddDry(weights.NativeHandle, ["\n", "。"], penaltyLastN: 32);
        foreach (LLamaToken token in new[] { 1000, 1001, 1002, 1000, 1001 })
        {
            dry.Accept(token);
        }

        var candidates = LLamaTokenDataArray.Create(new float[weights.NativeHandle.Vocab.Count]);
        using var pin = LLamaTokenDataArrayNative.Create(candidates, out var native);
        dry.Apply(ref native);
        Assert.True(native.Data[1002].Logit < 0);
        Assert.Equal(0f, native.Data[1003].Logit);
        Assert.All(candidates.Data.ToArray(), candidate => Assert.True(float.IsFinite(candidate.Logit)));
    }
}
