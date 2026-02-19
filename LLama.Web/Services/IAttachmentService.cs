using LLama.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace LLama.Web.Services;

public interface IAttachmentService
{
    string UploadsRoot { get; }
    Task<AttachmentUploadResult> SaveAsync(string connectionId, IEnumerable<IFormFile> files, CancellationToken cancellationToken);
    Task<AttachmentUploadResult> SaveAsync(string connectionId, IEnumerable<IBrowserFile> files, CancellationToken cancellationToken);
    IReadOnlyList<AttachmentInfo> GetAttachments(string connectionId, IEnumerable<string> ids);
    AttachmentInfo GetAttachment(string connectionId, string id);
    Task CleanupAsync(string connectionId);
}
