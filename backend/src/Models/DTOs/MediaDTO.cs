using kind_closet.Models.Enums;

namespace kind_closet.Models.DTOs;

// Media (MinIO)
public record UploadedMediaDto(string OriginalObjectKey, string ThumbnailObjectKey, string PreviewUrl, long FileSize, string FileHash);
