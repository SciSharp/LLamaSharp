using System.Text.Json;
using LLama;
using Microsoft.Extensions.Logging;

namespace LLava;

public class LLavaInteractExecutor : InteractiveExecutor
{
    
    protected LLavaWeights _weights;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="logger"></param>
    public LLavaInteractExecutor(LLamaContext context, LLavaWeights clip, ILogger? logger = null)
        : base(context, logger)
    {
        _weights = clip;
    }   

    protected override Task PreprocessInputs(string jsonPrompt, InferStateArgs args)
    {
        Prompt prompt = JsonSerializer.Deserialize<Prompt>(jsonPrompt);;

        if (prompt?.Image != null)
        {
            var result = base.PreprocessInputs("<image>", args);
            _weights.EmbedImage(Context, prompt.Image, out this._pastTokensCount);
        }

        return base.PreprocessInputs(prompt?.TextPrompt, args);

    }    
    
}