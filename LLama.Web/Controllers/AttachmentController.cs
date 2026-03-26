using LLama.Web.Models;
using LLama.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLama.Web.Controllers;

[ApiController]
[Route("api/attachments")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IModelSessionService _modelSessionService;

    public AttachmentController(IAttachmentService attachmentService, IModelSessionService modelSessionService)
    {
        _attachmentService = attachmentService;
        _modelSessionService = modelSessionService;
    }

    [HttpPost]
    [RequestSizeLimit(AttachmentService.MaxUploadSizeBytes)]
    public async Task<ActionResult<AttachmentUploadResult>> Upload([FromForm] string connectionId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return BadRequest("Missing connectionId.");

        if (files is null || files.Count == 0)
            return BadRequest("No files provided.");

        if (await _modelSessionService.GetAsync(connectionId) is null)
            return BadRequest("Unknown or inactive connectionId.");

        try
        {
            var result = await _attachmentService.SaveAsync(connectionId, files, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{connectionId}/{id}")]
    public async Task<ActionResult> Download(string connectionId, string id)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(id))
            return BadRequest("Missing connectionId or attachment id.");

        try
        {
            if (await _modelSessionService.GetAsync(connectionId) is null)
                return NotFound();

            var attachment = _attachmentService.GetAttachment(connectionId, id);
            if (attachment == null || string.IsNullOrWhiteSpace(attachment.FilePath))
                return NotFound();

            if (!System.IO.File.Exists(attachment.FilePath))
                return NotFound();

            var contentType = string.IsNullOrWhiteSpace(attachment.ContentType)
                ? "application/octet-stream"
                : attachment.ContentType;

            return PhysicalFile(attachment.FilePath, contentType, attachment.FileName);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
