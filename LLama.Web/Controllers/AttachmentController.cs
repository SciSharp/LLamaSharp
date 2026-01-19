using LLama.Web.Models;
using LLama.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace LLama.Web.Controllers;

[ApiController]
[Route("api/attachments")]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;

    public AttachmentController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    [HttpPost]
    [RequestSizeLimit(256_000_000)]
    public async Task<ActionResult<AttachmentUploadResult>> Upload([FromForm] string connectionId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return BadRequest("Missing connectionId.");

        if (files is null || files.Count == 0)
            return BadRequest("No files provided.");

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
    public ActionResult Download(string connectionId, string id)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(id))
            return BadRequest("Missing connectionId or attachment id.");

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
}
