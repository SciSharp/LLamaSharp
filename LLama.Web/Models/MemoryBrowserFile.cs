using Microsoft.AspNetCore.Components.Forms;

namespace LLama.Web.Models;

public sealed class MemoryBrowserFile : IBrowserFile
{
    private readonly byte[] _data;

    public MemoryBrowserFile(string name, string contentType, byte[] data, DateTimeOffset? lastModified = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("File name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(contentType))
            throw new ArgumentException("Content type is required.", nameof(contentType));
        _data = data ?? throw new ArgumentNullException(nameof(data));

        Name = name;
        ContentType = contentType;
        LastModified = lastModified ?? DateTimeOffset.UtcNow;
    }

    public string Name { get; }
    public DateTimeOffset LastModified { get; }
    public long Size => _data.LongLength;
    public string ContentType { get; }

    public Stream OpenReadStream(long maxAllowedSize = 512L * 1024 * 1024, CancellationToken cancellationToken = default)
    {
        if (Size > maxAllowedSize)
            throw new IOException($"File {Name} exceeds the maximum allowed size of {maxAllowedSize} bytes.");

        return new MemoryStream(_data, writable: false);
    }
}
