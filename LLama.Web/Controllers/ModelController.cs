using LLama.Web.Common;
using LLama.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LLama.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelController : ControllerBase
    {
        private readonly LLamaOptions _configuration;
        private readonly IModelService _modelService;

        public ModelController(IOptions<LLamaOptions> options, IModelService modelService)
        {
            _modelService = modelService;
            _configuration = options.Value;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetModels()
        {
            return Ok(_configuration.Models);
        }
    }
}
