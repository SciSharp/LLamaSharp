using System.Text;
using LLama.Common;
using LLama.Native;
using Xunit.Abstractions;

namespace LLama.Unittest;

public sealed class BeamTests
    : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ModelParams _params;
    private readonly LLamaWeights _model;

    public BeamTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _params = new ModelParams(Constants.GenerativeModelPath)
        {
            ContextSize = 2048,
            GpuLayerCount = 0,
        };
        _model = LLamaWeights.LoadFromFile(_params);
    }

    public void Dispose()
    {
        _model.Dispose();
    }

    //[Fact(Skip = "Very very slow in CI")]
    [Fact]
    public void BasicBeam()
    {
        const int num_beams = 2;
        const int n_predict = 3;
        const string prompt = "The cat sat on";

        var context = _model.CreateContext(_params);

        var initial_tokens = context.Tokenize(prompt);
        var batch = new LLamaBatch();
        batch.AddRange(initial_tokens, 0, LLamaSeqId.Zero, true);
        context.Decode(batch);

        var decoder = new StreamingTokenDecoder(context);
        NativeApi.llama_beam_search(context.NativeHandle, (data, state) =>
        {
            // Show the current state of every beam.
            for (var i = 0; i < state.Beams.Length; i++)
            {
                ref var view = ref state.Beams[i];

                var decoder = new StreamingTokenDecoder(context);
                decoder.AddRange(view.Tokens);
                var tokens = decoder.Read();

                _testOutputHelper.WriteLine($"B{i} ({view.CumulativeProbability}) => '{tokens}'");
            }

            // Once all beams agree on some tokens read them and append them to the output decoder
            if (state.CommonPrefixLength > 0)
            {
                var view = state.Beams[0];

                decoder.AddRange(view.Tokens.Slice(0, (int)state.CommonPrefixLength));
                
            }

        }, IntPtr.Zero, num_beams, initial_tokens.Length, n_predict, Math.Max(1, Environment.ProcessorCount / 2));

        _testOutputHelper.WriteLine($"Final: {prompt}{decoder.Read()}");
    }
}