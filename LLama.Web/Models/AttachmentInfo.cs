namespace LLama.Web.Models;

public enum AttachmentKind
{
    Unknown = 0,
    Pdf = 1,
    Image = 2,
    Audio = 3,
    Word = 4
}

public class AttachmentInfo
{
    public string Id { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string ConnectionId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string FilePath { get; set; }
    public AttachmentKind Kind { get; set; }
    public long SizeBytes { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public string ExtractedText { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public bool ExtractedTextTruncated { get; set; }
}

public class AttachmentUploadResult
{
    public List<AttachmentInfo> Attachments { get; set; } = new();
}
