using LLama.Common;
using LLama.WebAPI.Models;
using LLama.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLama.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILogger<ChatController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Send")]
        public Task<string> SendMessage([FromBody] SendMessageInput input, [FromServices] StatefulChatService _service)
        {
            return _service.Send(input);
        }

        [HttpPost("Send/Stream")]
        public async Task SendMessageStream([FromBody] SendMessageInput input, [FromServices] StatefulChatService _service, CancellationToken cancellationToken)
        {

            Response.ContentType = "text/event-stream";

            await foreach (var r in _service.SendStream(input))
            {
                await Response.WriteAsync("data:" + r + "\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            await Response.CompleteAsync();
        }

        [HttpPost("History")]
        public async Task<string> SendHistory([FromBody] HistoryInput input, [FromServices] StatelessChatService _service)
        {
            var history = new ChatHistory();

            var messages = input.Messages.Select(m => new ChatHistory.Message(Enum.Parse<AuthorRole>(m.Role), m.Content));

            history.Messages.AddRange(messages);

            return await _service.SendAsync(history);
        }
    }
}