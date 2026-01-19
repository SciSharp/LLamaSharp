using System.Collections.Concurrent;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LLama.Web.Models;
using Microsoft.AspNetCore.Components.Forms;
using UglyToad.PdfPig;

namespace LLama.Web.Services;

public class AttachmentService : IAttachmentService
{
    private const int MaxExtractedCharacters = 12000;
    private const long MaxUploadSize = 512L * 1024 * 1024;
    private readonly IWebHostEnvironment _environment;
    private readonly string _uploadsRoot;
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, AttachmentInfo>> _attachments = new();

    public AttachmentService(IWebHostEnvironment environment)
    {
        _environment = environment;
        var appDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        _uploadsRoot = Path.Combine(appDataRoot, "LLama.Web", "Uploads");
        Directory.CreateDirectory(_uploadsRoot);
    }

    public string UploadsRoot => _uploadsRoot;

    public async Task<AttachmentUploadResult> SaveAsync(string connectionId, IEnumerable<IFormFile> files, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentException("Connection id is required.", nameof(connectionId));

        ValidateUploads(files);

        var result = new AttachmentUploadResult();
        var storage = _attachments.GetOrAdd(connectionId, _ => new ConcurrentDictionary<string, AttachmentInfo>());
        var root = Path.Combine(_uploadsRoot, connectionId);
        Directory.CreateDirectory(root);

        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
                continue;

            var id = Guid.NewGuid().ToString("N");
            var safeName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(root, $"{id}-{safeName}");

            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var info = new AttachmentInfo
            {
                Id = id,
                ConnectionId = connectionId,
                FileName = safeName,
                FilePath = filePath,
                ContentType = file.ContentType,
                SizeBytes = file.Length,
                Kind = GetKind(file)
            };

            if (info.Kind == AttachmentKind.Pdf)
                ExtractPdfText(info);
            if (info.Kind == AttachmentKind.Word)
                ExtractWordText(info);

            storage[id] = info;
            result.Attachments.Add(info);
        }

        return result;
    }

    public async Task<AttachmentUploadResult> SaveAsync(string connectionId, IEnumerable<IBrowserFile> files, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentException("Connection id is required.", nameof(connectionId));

        ValidateUploads(files);

        var result = new AttachmentUploadResult();
        var storage = _attachments.GetOrAdd(connectionId, _ => new ConcurrentDictionary<string, AttachmentInfo>());
        var root = Path.Combine(_uploadsRoot, connectionId);
        Directory.CreateDirectory(root);

        foreach (var file in files)
        {
            if (file == null || file.Size == 0)
                continue;

            var id = Guid.NewGuid().ToString("N");
            var safeName = Path.GetFileName(file.Name);
            var filePath = Path.Combine(root, $"{id}-{safeName}");

            await using (var input = file.OpenReadStream(maxAllowedSize: MaxUploadSize, cancellationToken))
            await using (var output = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await input.CopyToAsync(output, cancellationToken);
            }

            var info = new AttachmentInfo
            {
                Id = id,
                ConnectionId = connectionId,
                FileName = safeName,
                FilePath = filePath,
                ContentType = file.ContentType,
                SizeBytes = file.Size,
                Kind = GetKind(file)
            };

            if (info.Kind == AttachmentKind.Pdf)
                ExtractPdfText(info);
            if (info.Kind == AttachmentKind.Word)
                ExtractWordText(info);

            storage[id] = info;
            result.Attachments.Add(info);
        }

        return result;
    }

    public IReadOnlyList<AttachmentInfo> GetAttachments(string connectionId, IEnumerable<string> ids)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || ids is null)
            return Array.Empty<AttachmentInfo>();

        if (!_attachments.TryGetValue(connectionId, out var storage))
            return Array.Empty<AttachmentInfo>();

        var list = new List<AttachmentInfo>();
        foreach (var id in ids)
        {
            if (storage.TryGetValue(id, out var info))
                list.Add(info);
        }

        return list;
    }

    public AttachmentInfo GetAttachment(string connectionId, string id)
    {
        if (string.IsNullOrWhiteSpace(connectionId) || string.IsNullOrWhiteSpace(id))
            return null;

        if (!_attachments.TryGetValue(connectionId, out var storage))
            return null;

        return storage.TryGetValue(id, out var info) ? info : null;
    }

    public Task CleanupAsync(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            return Task.CompletedTask;

        if (_attachments.TryRemove(connectionId, out _))
        {
            var root = Path.Combine(_uploadsRoot, connectionId);
            if (Directory.Exists(root))
                Directory.Delete(root, recursive: true);
        }

        return Task.CompletedTask;
    }

    private static AttachmentKind GetKind(IFormFile file)
    {
        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (contentType.Contains("pdf") || extension == ".pdf")
            return AttachmentKind.Pdf;
        if (IsWordDocument(contentType, extension))
            return AttachmentKind.Word;
        if (contentType.StartsWith("image/"))
            return AttachmentKind.Image;
        if (contentType.StartsWith("audio/") || extension is ".wav" or ".mp3" or ".m4a" or ".ogg" or ".flac" or ".webm")
            return AttachmentKind.Audio;

        return AttachmentKind.Unknown;
    }

    private static AttachmentKind GetKind(IBrowserFile file)
    {
        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        var extension = Path.GetExtension(file.Name).ToLowerInvariant();

        if (contentType.Contains("pdf") || extension == ".pdf")
            return AttachmentKind.Pdf;
        if (IsWordDocument(contentType, extension))
            return AttachmentKind.Word;
        if (contentType.StartsWith("image/"))
            return AttachmentKind.Image;
        if (contentType.StartsWith("audio/") || extension is ".wav" or ".mp3" or ".m4a" or ".ogg" or ".flac" or ".webm")
            return AttachmentKind.Audio;

        return AttachmentKind.Unknown;
    }

    private static bool IsWordDocument(string contentType, string extension)
    {
        if (extension == ".docx")
            return true;

        if (string.IsNullOrWhiteSpace(contentType))
            return false;

        return contentType.Contains("word") || contentType.Contains("officedocument.wordprocessingml");
    }

    private static bool IsAllowedUpload(string contentType, string extension)
    {
        if (extension == ".doc")
            return false;

        if (extension == ".docx" || contentType.Contains("officedocument.wordprocessingml") || contentType.Contains("word"))
            return true;

        if (extension == ".pdf" || contentType.Contains("pdf"))
            return true;

        if (contentType.StartsWith("image/"))
            return true;

        if (contentType.StartsWith("audio/"))
            return true;

        if (extension is ".wav" or ".mp3" or ".m4a" or ".ogg" or ".flac" or ".webm")
            return true;

        return false;
    }

    private static void ValidateUploads(IEnumerable<IFormFile> files)
    {
        var invalid = files
            .Where(file => file != null)
            .Where(file => !IsAllowedUpload(file.ContentType?.ToLowerInvariant() ?? string.Empty, Path.GetExtension(file.FileName).ToLowerInvariant()))
            .Select(file => file.FileName)
            .ToList();

        if (invalid.Count == 0)
            return;

        throw new InvalidOperationException($"Unsupported files: {string.Join(", ", invalid)}. Use PDF, DOCX, or images.");
    }

    private static void ValidateUploads(IEnumerable<IBrowserFile> files)
    {
        var invalid = files
            .Where(file => file != null)
            .Where(file => !IsAllowedUpload(file.ContentType?.ToLowerInvariant() ?? string.Empty, Path.GetExtension(file.Name).ToLowerInvariant()))
            .Select(file => file.Name)
            .ToList();

        if (invalid.Count == 0)
            return;

        throw new InvalidOperationException($"Unsupported files: {string.Join(", ", invalid)}. Use PDF, DOCX, or images.");
    }

    private static void ExtractPdfText(AttachmentInfo info)
    {
        if (!File.Exists(info.FilePath))
            return;

        var builder = new StringBuilder();
        try
        {
            using var document = PdfDocument.Open(info.FilePath);
            foreach (var page in document.GetPages())
            {
                if (builder.Length >= MaxExtractedCharacters)
                    break;

                builder.AppendLine(page.Text);
            }

            var text = builder.ToString();
            if (text.Length > MaxExtractedCharacters)
            {
                info.ExtractedText = text.Substring(0, MaxExtractedCharacters);
                info.ExtractedTextTruncated = true;
            }
            else
            {
                info.ExtractedText = text;
                info.ExtractedTextTruncated = false;
            }
        }
        catch
        {
            info.ExtractedText = string.Empty;
            info.ExtractedTextTruncated = false;
        }
    }

    private static void ExtractWordText(AttachmentInfo info)
    {
        if (!File.Exists(info.FilePath))
            return;

        var extension = Path.GetExtension(info.FilePath).ToLowerInvariant();
        if (extension != ".docx")
        {
            info.ExtractedText = string.Empty;
            info.ExtractedTextTruncated = false;
            return;
        }

        try
        {
            using var archive = ZipFile.OpenRead(info.FilePath);
            var entry = archive.GetEntry("word/document.xml");
            if (entry == null)
                return;

            using var stream = entry.Open();
            var document = XDocument.Load(stream);
            var textNodes = document.Descendants()
                .Where(node => node.Name.LocalName == "t")
                .Select(node => node.Value);

            var builder = new StringBuilder();
            foreach (var text in textNodes)
            {
                if (builder.Length >= MaxExtractedCharacters)
                    break;

                builder.Append(text);
                builder.AppendLine();
            }

            var resultText = builder.ToString();
            if (resultText.Length > MaxExtractedCharacters)
            {
                info.ExtractedText = resultText.Substring(0, MaxExtractedCharacters);
                info.ExtractedTextTruncated = true;
            }
            else
            {
                info.ExtractedText = resultText;
                info.ExtractedTextTruncated = false;
            }
        }
        catch
        {
            info.ExtractedText = string.Empty;
            info.ExtractedTextTruncated = false;
        }
    }
}
