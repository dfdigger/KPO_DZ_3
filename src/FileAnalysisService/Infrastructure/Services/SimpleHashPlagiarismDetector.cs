using System.Security.Cryptography;
using FileAnalysisService.Domain.Entities;
using FileAnalysisService.Domain.Services;
using FileAnalysisService.Infrastructure.Clients;
using FileAnalysisService.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace FileAnalysisService.Infrastructure.Services;

public class SimpleHashPlagiarismDetector : IPlagiarismDetector
{
    private readonly AnalysisDbContext _dbContext;
    private readonly IFileStorageClient _fileStorageClient;

    public SimpleHashPlagiarismDetector(
        AnalysisDbContext dbContext,
        IFileStorageClient fileStorageClient)
    {
        _dbContext = dbContext;
        _fileStorageClient = fileStorageClient;
    }

    public Report Analyze(Work work)
    {
        // скачиваем файл из FileStorageService
        using var stream = _fileStorageClient.DownloadFile(work.FileId);

        // считаем хеш содержимого файла
        var hash = ComputeSha256(stream);

        // ищем в БД более старые работы с таким же хешем,
        // но от другого студента и по тому же заданию
        var isPlagiarism = _dbContext.Reports
            .Include(r => r.Work)
            .Any(r =>
                r.FileHash == hash &&
                r.Work.AssignmentId == work.AssignmentId &&
                r.Work.StudentId != work.StudentId &&
                r.Work.SubmittedAt < work.SubmittedAt);

        // создаём отчёт
        var report = new Report
        {
            Id = Guid.NewGuid(),
            WorkId = work.Id,
            FileHash = hash,
            IsPlagiarism = isPlagiarism,
            CheckedAt = DateTime.UtcNow
        };

        return report;
    }

    private static string ComputeSha256(Stream stream)
    {
        stream.Position = 0;
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }
}