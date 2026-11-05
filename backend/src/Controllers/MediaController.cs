using kind_closet.Data;
using kind_closet.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kind_closet.Controllers;



[ApiController]
[Route("api/v1/media")]
[Authorize]
public class MediaController : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(50_000_000)]
    public async Task<ActionResult<List<UploadedMediaDto>>> UploadImages(
        [FromForm] List<IFormFile> files,
        CancellationToken ct)
    {
        if (files.Count is < 1 or > 10)
            return BadRequest("Допускается загрузка до 10 изображений за раз.");

        var result = new List<UploadedMediaDto>();
        foreach (var file in files)
        {
            var imageId = Guid.NewGuid();
            var originalKey = $"listings/originals/{imageId}.webp";
            var thumbKey = $"listings/thumbs/{imageId}_thumb.webp";

            // TODO: Сжатие и генерация превью через ImageSharp + отправка в MinIO бакет good-wardrobe-images
            result.Add(new UploadedMediaDto(originalKey, thumbKey, $"/media/{thumbKey}", file.Length, "sha256-hash"));
        }

        return Ok(result);
    }
}