using FileStorageService.Domain.Services;
using FileStorageService.Infrastructure.Db;
using FileStorageService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<StorageDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IFileStorage, LocalFileStorage>();

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
    var db = scope.ServiceProvider.GetRequiredService<StorageDbContext>();
    db.Database.Migrate(); // применит все миграции, создаст таблицы, если их нет
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();