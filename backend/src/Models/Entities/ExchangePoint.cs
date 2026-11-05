using System.ComponentModel.DataAnnotations;
using kind_closet.Models.Enums;

namespace kind_closet.Models.Entities;

public class ExchangePoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizerId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    [Required, MaxLength(300)]
    public string Address { get; set; } = string.Empty;
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Хранятся как jsonb в PostgreSQL
    public string WorkingHoursJson { get; set; } = "{}";
    public string ContactsJson { get; set; } = "{}";
    
    // Массив поддерживаемых форматов обмена и категорий
    public ExchangeFormat[] SupportedFormats { get; set; } = Array.Empty<ExchangeFormat>();
    public int[] AcceptedCategoryIds { get; set; } = Array.Empty<int>();

    public PointStatus Status { get; set; } = PointStatus.Active;
    public bool IsPartner { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Organizer { get; set; } = null!;
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
