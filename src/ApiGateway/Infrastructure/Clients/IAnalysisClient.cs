namespace ApiGateway.Infrastructure.Clients;

public interface IAnalysisClient
{
    ReportDto StartAnalysis(StartAnalysisRequest request);
    List<ReportDto> GetReports(Guid workId);
}

public class StartAnalysisRequest
{
    public Guid StudentId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid FileId { get; set; }
}

public class ReportDto
{
    public Guid Id { get; set; }
    public Guid WorkId { get; set; }
    public bool IsPlagiarism { get; set; }
    public string FileHash { get; set; } = null!;
    public DateTime CheckedAt { get; set; }
}