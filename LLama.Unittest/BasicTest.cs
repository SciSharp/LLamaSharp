using LLama.Common;

namespace LLama.Unittest
{
    public class BasicTest
    {
        [Fact]
        public void LoadModel()
        {
            var model = new LLamaModel(new ModelParams("Models/llama-2-7b-chat.ggmlv3.q3_K_S.bin", contextSize: 256));
            model.Dispose();
        }
    }
}