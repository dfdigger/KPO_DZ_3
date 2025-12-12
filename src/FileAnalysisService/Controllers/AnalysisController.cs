using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Services;
using FileAnalysisService.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc;

namespace FileAnalysisService.Controllers;

[ApiController]
[Route("analysis")]
public class AnalysisController : ControllerBase
{
    private readonly AnalysisDbContext _dbContext;
    private readonly IPlagiarismDetector _plagiarismDetector;

    public AnalysisController(
        AnalysisDbContext dbContext,
        IPlagiarismDetector plagiarismDetector)
    {
        _dbContext = dbContext;
        _plagiarismDetector = plagiarismDetector;
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
        public string FileHash { get; set; } = default!;
        public DateTime CheckedAt { get; set; }
    }

    /// <summary>
    /// запустить анализ работы. Создаёт Work и Report, возвращает отчёт
    /// </summary>
    [HttpPost("start")]
    public IActionResult StartAnalysis([FromBody] StartAnalysisRequest request)
    {
        if (request.StudentId == Guid.Empty ||
            request.AssignmentId == Guid.Empty ||
            request.FileId == Guid.Empty)
        {
            return BadRequest("StudentId, AssignmentId и FileId обязательны");
        }

        var work = new Work
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            AssignmentId = request.AssignmentId,
            FileId = request.FileId,
            SubmittedAt = DateTime.UtcNow
        };

        _dbContext.Works.Add(work);
        _dbContext.SaveChanges();

        var report = _plagiarismDetector.Analyze(work);

        _dbContext.Reports.Add(report);
        _dbContext.SaveChanges();

        var dto = new ReportDto
        {
            Id = report.Id,
            WorkId = report.WorkId,
            IsPlagiarism = report.IsPlagiarism,
            FileHash = report.FileHash,
            CheckedAt = report.CheckedAt
        };

        return Ok(dto);
    }

    /// <summary>
    /// получить все отчёты по работе
    /// </summary>
    [HttpGet("works/{workId:guid}/reports")]
    public IActionResult GetReports(Guid workId)
    {
        var reports = _dbContext.Reports
            .Where(r => r.WorkId == workId)
            .OrderBy(r => r.CheckedAt)
            .Select(r => new ReportDto
            {
                Id = r.Id,
                WorkId = r.WorkId,
                IsPlagiarism = r.IsPlagiarism,
                FileHash = r.FileHash,
                CheckedAt = r.CheckedAt
            })
            .ToList();

        if (reports.Count == 0)
            return NotFound();

        return Ok(reports);
    }
}