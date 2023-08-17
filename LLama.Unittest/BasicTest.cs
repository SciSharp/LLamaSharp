using System.Text;
using LLama.Common;

namespace LLama.Unittest
{
    public class BasicTest
        : IDisposable
    {
        private readonly ModelParams _params;
        private readonly LLamaWeights _model;

        public BasicTest()
        {
            _params = new ModelParams("Models/llama-2-7b-chat.ggmlv3.q3_K_S.bin", contextSize: 2048);
            _model = LLamaWeights.LoadFromFile(_params);
        }

        public void Dispose()
        {
            _model.Dispose();
        }

        [Fact]
        public void LoadModel()
        {
            var model = _model.CreateContext(_params, Encoding.UTF8);
            model.Dispose();
        }
    }
}