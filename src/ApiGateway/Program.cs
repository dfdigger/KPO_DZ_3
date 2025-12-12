using ApiGateway.Infrastructure.Clients;

var builder = WebApplication.CreateBuilder(args);

var fileStorageUrl = builder.Configuration["Services:FileStorageServiceUrl"];
var analysisServiceUrl = builder.Configuration["Services:FileAnalysisServiceUrl"];

// HttpClient для FileStorageService
builder.Services.AddHttpClient<IFileStorageClient, FileStorageClient>(client =>
{
    client.BaseAddress = new Uri(fileStorageUrl);
});

// HttpClient для FileAnalysisService
builder.Services.AddHttpClient<IAnalysisClient, AnalysisClient>(client =>
{
    client.BaseAddress = new Uri(analysisServiceUrl);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();