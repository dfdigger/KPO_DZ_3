namespace ApiGateway.Infrastructure.Clients;

public interface IFileStorageClient
{
    Guid UploadFile(IFormFile file);
}