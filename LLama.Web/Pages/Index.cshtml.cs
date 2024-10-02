using LLama.Sampling;
using LLama.Web.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace LLama.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, IOptions<LLamaOptions> options)
        {
            _logger = logger;
            Options = options.Value;
        }

        public LLamaOptions Options { get; set; }

        [BindProperty]
        public ISessionConfig SessionConfig { get; set; }

        [BindProperty]
        public InferenceOptions InferenceOptions { get; set; }

        public void OnGet()
        {
            SessionConfig = new SessionConfig
            {
                Prompt = "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
                AntiPrompt = "User:",
                OutputFilter = "User:, Assistant: "
            };

            InferenceOptions = new InferenceOptions
            {
                SamplingPipeline = new DefaultSamplingPipeline
                {
                    Temperature = 0.8f
                },
            };
        }
    }
}