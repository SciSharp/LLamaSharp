using LLama.Common;
using Microsoft.AspNetCore.Http;
using System.Text;
using static LLama.LLamaTransforms;

namespace LLama.WebAPI.Services
{
    public class StatelessChatService
    {
        private readonly LLamaModel _model;
        private readonly ChatSession _session;

        public StatelessChatService(IConfiguration configuration)
        {
            _model = new LLamaModel(new ModelParams(configuration["ModelPath"], contextSize: 512));
            // TODO: replace with a stateless executor
            _session = new ChatSession(new InteractiveExecutor(_model))
                        .WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(new string[] { "User:", "Assistant:" }, redundancyLength: 8))
                        .WithHistoryTransform(new HistoryTransform());
        }

        public async Task<string> SendAsync(ChatHistory history)
        {
            var result = _session.ChatAsync(history, new InferenceParams()
            {
                AntiPrompts = new string[] { "User:" },
            });

            var sb = new StringBuilder();
            await foreach (var r in result)
            {
                Console.Write(r);
                sb.Append(r);
            }

            return sb.ToString();

        }
    }
    public class HistoryTransform : DefaultHistoryTransform
    {
        public override string HistoryToText(ChatHistory history)
        {
            return base.HistoryToText(history) + "\n Assistant:";
        }

    }
}
