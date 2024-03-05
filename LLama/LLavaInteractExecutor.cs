using LLama.Native;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LLama
{
    public class LLavaInteractExecutor /*: InteractiveExecutor*/
    {
        /// <summary>
        /// weights of LLava model
        /// </summary>
        protected SafeLlavaModelHandle handle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        //public LLavaInteractExecutor(SafeLlavaModelHandle handel, ILogger? logger = null)
        //{
        //    this.handle = handel;
        //    this.logger = logger;
        //}

        //protected override Task PreprocessInputs(string prompt, byte[] imageByte, InferStateArgs args)
        //{
        //    if (_is_prompt_run)
        //    {
        //        // When running the first input (prompt) in inteactive mode, we should specially process it.
        //        _embed_inps = Context.Tokenize(text, true).ToList();
        //    }
        //    else
        //    {
        //        if (!text.EndsWith("\n"))
        //        {
        //            text += "\n";
        //        }
        //        var line_inp = Context.Tokenize(text, false);
        //        _embed_inps.AddRange(line_inp);
        //        args.RemainedTokens -= line_inp.Length;
        //    }

        //    return Task.CompletedTask;

        //}
    }
}
