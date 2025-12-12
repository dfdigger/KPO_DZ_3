using FileStorageService.Domain.Entities;
using FileStorageService.Domain.Services;
using FileStorageService.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Services;

public class LocalFileStorage : IFileStorage
    {
        private readonly StorageDbContext _dbContext;
        private readonly string _rootPath;

        public LocalFileStorage(
            StorageDbContext dbContext,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _dbContext = dbContext;

            var configuredPath = configuration["FileStorage:RootPath"];
            _rootPath = string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine(env.ContentRootPath, "Files")
                : configuredPath;

            Directory.CreateDirectory(_rootPath);
        }

        public FileMetadata Save(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("File is empty");

            var id = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            var storedName = id.ToString("N") + extension;
            var path = Path.Combine(_rootPath, storedName);

            using (var stream = new FileStream(path, FileMode.CreateNew))
            {
                file.CopyTo(stream);
            }

            var meta = new FileMetadata
            {
                Id = id,
                OriginalName = file.FileName,
                StoredName = storedName,
                ContentType = file.ContentType,
                Size = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _dbContext.Files.Add(meta);
            _dbContext.SaveChanges();

            return meta;
        }

        public (FileMetadata Meta, Stream Content) Get(Guid id)
        {
            var meta = _dbContext.Files.FirstOrDefault(x => x.Id == id);
            if (meta == null)
                throw new FileNotFoundException("File metadata not found", id.ToString());

            var path = Path.Combine(_rootPath, meta.StoredName);
            if (!File.Exists(path))
                throw new FileNotFoundException("File content not found on disk", path);

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (meta, stream);
        }
    }