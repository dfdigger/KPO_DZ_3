using System.Net.Http.Headers;
using System.Text.Json;

namespace ApiGateway.Infrastructure.Clients;

public class FileStorageClient : IFileStorageClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions =
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    private class FileUploadResponse
    {
        public Guid FileId { get; set; }
        public string OriginalName { get; set; } = default!;
        public long Size { get; set; }
    }

    public FileStorageClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Guid UploadFile(IFormFile file)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        var response = _httpClient.PostAsync("files", content).Result;
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().Result;
        var dto = JsonSerializer.Deserialize<FileUploadResponse>(json, JsonOptions)
                  ?? throw new InvalidOperationException("Invalid response from FileStorageService");

        return dto.FileId;
    }
}