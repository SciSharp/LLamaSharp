using System.Text;
using LLama.Common;
using LLama.Native;
using Xunit.Abstractions;

namespace LLama.Unittest
{
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

        [Fact]
        public void BasicBeam()
        {
            const int num_beams = 3;
            const int n_predict = 11;

            var context = _model.CreateContext(_params);

            var result = new StringBuilder();

            var initial_tokens = context.Tokenize("The cat sat on top of");
            result.Append(context.DeTokenize(initial_tokens.ToArray()));
            context.Eval(initial_tokens, 0);

            NativeApi.llama_beam_search(context.NativeHandle, (data, state) =>
            {
                for (var i = 0; i < state.BeamViews.Length; i++)
                {
                    var view = state.BeamViews[i];
                    var tokens = context.DeTokenize(view.Tokens.ToArray());
                    _testOutputHelper.WriteLine($"B{i} ({view.CumulativeProbability}) => '{tokens}'");
                }

                if (state.CommonPrefixLength > 0)
                {
                    var view = state.BeamViews[0];
                    result.Append(context.DeTokenize(view.Tokens.Slice(0, (int)state.CommonPrefixLength).ToArray()));
                }

            }, IntPtr.Zero, num_beams, initial_tokens.Length, n_predict, 8);

            _testOutputHelper.WriteLine($"Final: {result}");
        }
    }
}
