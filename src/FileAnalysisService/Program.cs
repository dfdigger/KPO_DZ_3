using FileAnalysisService.Domain.Services;
using FileAnalysisService.Infrastructure.Clients;
using FileAnalysisService.Infrastructure.Db;
using FileAnalysisService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// строка подключения к БД анализа
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// EF Core
builder.Services.AddDbContext<AnalysisDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// HttpClient для обращения к FileStorageService
builder.Services.AddHttpClient<IFileStorageClient, FileStorageClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:FileStorageServiceUrl"]);
});

// плагиат-детектор
builder.Services.AddScoped<IPlagiarismDetector, SimpleHashPlagiarismDetector>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AnalysisDbContext>();
    db.Database.Migrate(); // применит все миграции, создаст таблицы, если их нет
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();