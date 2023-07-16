using LLama.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace LLama.Web.Pages
{
    public class InteractiveModel : PageModel
    {
        private readonly ILogger<InteractiveModel> _logger;

        public InteractiveModel(ILogger<InteractiveModel> logger, IOptions<LLamaOptions> options)
        {
            _logger = logger;
            Options = options.Value;
        }

        public LLamaOptions Options { get; set; }

        public void OnGet()
        {
        }
    }
}