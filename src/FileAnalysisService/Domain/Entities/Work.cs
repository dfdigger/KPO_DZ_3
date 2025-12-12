namespace FileAnalysisService.Domain.Entities;

public class Work
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid FileId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public List<Report> Reports { get; set; } = new();
}