using System.ComponentModel.DataAnnotations;
using kind_closet.Models.Enums;

namespace kind_closet.Models.Entities;

public class ListingImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ListingId { get; set; }

    // Ключи объектов в бакете good-wardrobe-images (MinIO)
    [Required, MaxLength(512)]
    public string OriginalObjectKey { get; set; } = string.Empty;
    
    [Required, MaxLength(512)]
    public string ThumbnailObjectKey { get; set; } = string.Empty;

    public long FileSize { get; set; }
    
    [Required, MaxLength(64)]
    public string FileHash { get; set; } = string.Empty;
    
    public int SortOrder { get; set; }

    public Listing Listing { get; set; } = null!;
}