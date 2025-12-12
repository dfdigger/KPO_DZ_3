namespace FileAnalysisService.Domain.Entities;

public class Report
{
    public Guid Id { get; set; }
    public Guid WorkId { get; set; }
    public Work Work { get; set; } = null!;
    public string FileHash { get; set; } = null!;
    public bool IsPlagiarism { get; set; }
    public DateTime CheckedAt { get; set; }
}