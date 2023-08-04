using LLama.Web.Common;
using LLama.Web.Models;
using LLama.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace LLama.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ConnectionSessionService _modelSessionService;

        public IndexModel(ILogger<IndexModel> logger, IOptions<LLamaOptions> options, ConnectionSessionService modelSessionService)
        {
            _logger = logger;
            Options = options.Value;
            _modelSessionService = modelSessionService;
        }

        public LLamaOptions Options { get; set; }

        [BindProperty]
        public CreateSessionModel SessionOptions { get; set; }

        public void OnGet()
        {
            SessionOptions = new CreateSessionModel
            {
                Prompt = "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
                AntiPrompt = "User:",
                OutputFilter = "User:, Response:"
            };
        }

        public async Task<IActionResult> OnPostCancel(CancelModel model)
        {
            await _modelSessionService.CancelAsync(model.ConnectionId);
            return new JsonResult(default);
        }
    }
}