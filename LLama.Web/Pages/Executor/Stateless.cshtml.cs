using LLama.Web.Common;
using LLama.Web.Models;
using LLama.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace LLama.Web.Pages
{
    public class StatelessModel : PageModel
    {
        private readonly ILogger<StatelessModel> _logger;
        private readonly ConnectionSessionService _modelSessionService;

        public StatelessModel(ILogger<StatelessModel> logger, IOptions<LLamaOptions> options, ConnectionSessionService modelSessionService)
        {
            _logger = logger;
            Options = options.Value;
            _modelSessionService = modelSessionService;
        }

        public LLamaOptions Options { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCancel(CancelModel model)
        {
            await _modelSessionService.CancelAsync(model.ConnectionId);
            return new JsonResult(default);
        }
    }
}