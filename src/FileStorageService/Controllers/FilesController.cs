using FileStorageService.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStorageService.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    private readonly IFileStorage _fileStorage;

    public FilesController(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    /// <summary>
    /// загружает файл, возвращает fileId
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(100_000_000)] // 100 MB
    public IActionResult Upload([FromForm] IFormFile file)
    {
        if (file == null!)
            return BadRequest("File is required");

        var meta = _fileStorage.Save(file);

        return Ok(new
        {
            fileId = meta.Id,
            originalName = meta.OriginalName,
            size = meta.Size
        });
    }

    /// <summary>
    /// скачать файл по идентификатору
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult Download(Guid id)
    {
        try
        {
            var (meta, content) = _fileStorage.Get(id);
            // stream будет закрыт фреймворком, когда ответ отправится
            return File(content, meta.ContentType, meta.OriginalName);
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
    }
}