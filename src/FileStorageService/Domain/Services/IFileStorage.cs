using FileStorageService.Domain.Entities;

namespace FileStorageService.Domain.Services;

public interface IFileStorage
{
    FileMetadata Save(IFormFile file);
    (FileMetadata Meta, Stream Content) Get(Guid id);
}