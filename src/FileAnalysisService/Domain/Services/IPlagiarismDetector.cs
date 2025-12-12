using FileAnalysisService.Domain.Entities;

namespace FileAnalysisService.Domain.Services;

public interface IPlagiarismDetector
{
    Report Analyze(Work work);
}