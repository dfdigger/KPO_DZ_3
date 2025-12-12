using System.Text;
using System.Text.Json;

namespace ApiGateway.Infrastructure.Clients;

public class AnalysisClient : IAnalysisClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions =
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    public AnalysisClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public ReportDto StartAnalysis(StartAnalysisRequest request)
    {
        var jsonBody = JsonSerializer.Serialize(request);
        using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        var response = _httpClient.PostAsync("analysis/start", content).Result;
        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().Result;
        var dto = JsonSerializer.Deserialize<ReportDto>(json, JsonOptions)
                  ?? throw new InvalidOperationException("Invalid response from FileAnalysisService");

        return dto;
    }

    public List<ReportDto> GetReports(Guid workId)
    {
        var response = _httpClient.GetAsync($"analysis/works/{workId}/reports").Result;

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return new List<ReportDto>();

        response.EnsureSuccessStatusCode();

        var json = response.Content.ReadAsStringAsync().Result;
        var dtos = JsonSerializer.Deserialize<List<ReportDto>>(json, JsonOptions)
                   ?? new List<ReportDto>();

        return dtos;
    }
}