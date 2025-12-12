namespace FileStorageService.Domain.Entities;

public class FileMetadata
{
    public Guid Id { get; set; }
    public string OriginalName { get; set; } = null!;
    public string StoredName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}