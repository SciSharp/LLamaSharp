using LLama.Common;
using Xunit.Abstractions;

namespace LLama.Unittest
{
    public sealed class ChatSessionTests
        : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly ModelParams _params;
        private readonly LLamaWeights _model;

        public ChatSessionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _params = new ModelParams(Constants.GenerativeModelPath2)
            {
                ContextSize = 128,
                GpuLayerCount = Constants.CIGpuLayerCount
            };
            _model = LLamaWeights.LoadFromFile(_params);
        }

        public void Dispose()
        {
            _model.Dispose();
        }

      
    }
}