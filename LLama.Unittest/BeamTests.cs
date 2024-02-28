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
        _params = new ModelParams(Constants.ModelPath)
        {
            ContextSize = 2048
        };
        _model = LLamaWeights.LoadFromFile(_params);
    }

    public void Dispose()
    {
        _model.Dispose();
    }

    [Fact(Skip = "Very very slow in CI")]
    public void BasicBeam()
    {
        const int num_beams = 2;
        const int n_predict = 3;
        const string prompt = "The cat sat on";

        var context = _model.CreateContext(_params);

        var result = new StringBuilder();

        var initial_tokens = context.Tokenize(prompt);
        result.Append(prompt);
        //context.Eval(initial_tokens.AsSpan(), 0);
        throw new NotImplementedException("Replace Eval");

        NativeApi.llama_beam_search(context.NativeHandle, (data, state) =>
        {
            for (var i = 0; i < state.Beams.Length; i++)
            {
                ref var view = ref state.Beams[i];

                var decoder = new StreamingTokenDecoder(context);
                decoder.AddRange(view.Tokens);
                var tokens = decoder.Read();

                _testOutputHelper.WriteLine($"B{i} ({view.CumulativeProbability}) => '{tokens}'");
            }

            if (state.CommonPrefixLength > 0)
            {
                var view = state.Beams[0];

                var decoder = new StreamingTokenDecoder(context);
                decoder.AddRange(view.Tokens.Slice(0, (int)state.CommonPrefixLength));
                var tokens = decoder.Read();

                result.Append(tokens);
                
            }

        }, IntPtr.Zero, num_beams, initial_tokens.Length, n_predict, Math.Max(1, Environment.ProcessorCount / 2));

        _testOutputHelper.WriteLine($"Final: {result}");
    }
}