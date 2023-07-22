using LLama.Web.Models;
using LLama.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace LLama.Web.Pages
{
    public class InteractiveModel : PageModel
    {
        private readonly ILogger<InteractiveModel> _logger;
        private readonly IModelSessionService _modelSessionService;

        public InteractiveModel(ILogger<InteractiveModel> logger, IOptions<LLamaOptions> options, IModelSessionService modelSessionService)
        {
            _logger = logger;
            Options = options.Value;
            _modelSessionService = modelSessionService;
        }

        public LLamaOptions Options { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCancel([FromBody]CancelModel model)
        {
            await _modelSessionService.CancelAsync(model.ConnectionId);
            return new JsonResult(default);
        }
    }
}