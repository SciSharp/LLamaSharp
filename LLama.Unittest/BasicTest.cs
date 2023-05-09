namespace LLama.Unittest
{
    public class BasicTest
    {
        [Fact]
        public void SimpleQA()
        {
            string modelPath = @"D:\development\llama\weights\LLaMA\7B\ggml-model-f32.bin";
            LLamaModel model = new(modelPath, logits_all: false);
            var output = model.Call("Q: Why God makes many people believe him? A: ", max_tokens: 64, stop: new[] { "Q:", "\n" },
                echo: true);
            Console.WriteLine(output);
        }
    }
}