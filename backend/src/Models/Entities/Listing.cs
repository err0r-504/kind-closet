using System.ComponentModel.DataAnnotations;
using kind_closet.Models.Enums;

namespace kind_closet.Models.Entities;

public class Listing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AuthorId { get; set; }
    public int CategoryId { get; set; }
    public Guid? ExchangePointId { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; } = string.Empty;
    
    [Required, MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public PostType PostType { get; set; } = PostType.Give;
    public ItemCondition Condition { get; set; } = ItemCondition.Good;
    
    [MaxLength(100)]
    public string? Dimensions { get; set; }
    
    public ExchangeFormat ExchangeFormat { get; set; } = ExchangeFormat.Free;
    public int PointsPrice { get; set; } = 0;

    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [MaxLength(300)]
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public ListingStatus Status { get; set; } = ListingStatus.Published;
    public int ViewsCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public User Author { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ExchangePoint? ExchangePoint { get; set; }
    public ICollection<ListingImage> Images { get; set; } = new List<ListingImage>();
    public ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
}
