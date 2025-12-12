using ApiGateway.Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("works")]
public class WorksController : ControllerBase
{
    private readonly IFileStorageClient _fileStorageClient;
    private readonly IAnalysisClient _analysisClient;

    public WorksController(
        IFileStorageClient fileStorageClient,
        IAnalysisClient analysisClient)
    {
        _fileStorageClient = fileStorageClient;
        _analysisClient = analysisClient;
    }

    public class UploadWorkRequest
    {
        public Guid StudentId { get; set; }
        public Guid AssignmentId { get; set; }
        public IFormFile File { get; set; } = null!;
    }

    public class UploadWorkResponse
    {
        public Guid WorkId { get; set; }
        public Guid FileId { get; set; }
        public bool IsPlagiarism { get; set; }
        public string FileHash { get; set; } = null!;
        public DateTime CheckedAt { get; set; }
    }

    /// <summary>
    /// загрузить работу и сразу запустить анализ
    /// </summary>
    [HttpPost("upload")]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Upload([FromForm] UploadWorkRequest request)
    {
        if (request.StudentId == Guid.Empty ||
            request.AssignmentId == Guid.Empty ||
            request.File == null!)
        {
            return BadRequest("StudentId, AssignmentId и файл обязательны");
        }

        // загружаем файл в FileStorageService
        var fileId = _fileStorageClient.UploadFile(request.File);

        // запускаем анализ в FileAnalysisService
        var analysisRequest = new StartAnalysisRequest
        {
            StudentId = request.StudentId,
            AssignmentId = request.AssignmentId,
            FileId = fileId
        };

        var report = _analysisClient.StartAnalysis(analysisRequest);

        var response = new UploadWorkResponse
        {
            WorkId = report.WorkId,
            FileId = fileId,
            IsPlagiarism = report.IsPlagiarism,
            FileHash = report.FileHash,
            CheckedAt = report.CheckedAt
        };

        return Ok(response);
    }

    /// <summary>
    /// получить отчёты по работе (для преподавателя).
    /// </summary>
    [HttpGet("{workId:guid}/reports")]
    public IActionResult GetReports(Guid workId)
    {
        var reports = _analysisClient.GetReports(workId);

        if (reports.Count == 0)
            return NotFound();

        return Ok(reports);
    }
}