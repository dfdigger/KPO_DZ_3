namespace FileAnalysisService.Infrastructure.Clients;

public class FileStorageClient : IFileStorageClient
{
    private readonly HttpClient _httpClient;

    public FileStorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Stream DownloadFile(Guid fileId)
    {
        var response = _httpClient.GetAsync($"files/{fileId}").Result;
        response.EnsureSuccessStatusCode();
        return response.Content.ReadAsStreamAsync().Result;
    }
}