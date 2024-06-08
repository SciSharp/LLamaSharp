using LLama.Common;
using LLama.Transformers;

namespace LLama.Unittest.Transformers;

public class PromptTemplateTransformerTests
{
    private readonly LLamaWeights _model;
    private readonly PromptTemplateTransformer TestableTransformer;

    public PromptTemplateTransformerTests()
    {
        var @params = new ModelParams(Constants.GenerativeModelPath)
        {
            ContextSize = 1,
            GpuLayerCount = Constants.CIGpuLayerCount
        };
        _model = LLamaWeights.LoadFromFile(@params);

        TestableTransformer = new PromptTemplateTransformer(_model, true);
    }

    [Fact]
    public void HistoryToText_EncodesCorrectly()
    {
        const string userData = nameof(userData);
        var template = TestableTransformer.HistoryToText(new ChatHistory(){
            Messages = [new ChatHistory.Message(AuthorRole.User, userData)]
        });

        const string expected = "<|im_start|>user\n" +
                                $"{userData}<|im_end|>\n" +
                                "<|im_start|>assistant\n";
        Assert.Equal(expected, template);
    }
}
