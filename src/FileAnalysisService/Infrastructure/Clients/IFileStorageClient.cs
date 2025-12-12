namespace FileAnalysisService.Infrastructure.Clients;

public interface IFileStorageClient
{
    Stream DownloadFile(Guid fileId);
}