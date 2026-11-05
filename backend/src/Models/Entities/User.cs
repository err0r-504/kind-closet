using System.ComponentModel.DataAnnotations;
using kind_closet.Models.Enums;

namespace kind_closet.Models.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [MaxLength(256)]
    public string? Email { get; set; }
    public bool IsEmailConfirmed { get; set; }
    
    [MaxLength(32)]
    public string? Phone { get; set; }
    public bool IsPhoneConfirmed { get; set; }
    
    public string? PasswordHash { get; set; }
    public OAuthProvider OAuthProvider { get; set; } = OAuthProvider.None;
    
    [MaxLength(128)]
    public string? OAuthId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(512)]
    public string? AvatarObjectKey { get; set; }
    
    [Required, MaxLength(100)]
    public string City { get; set; } = string.Empty;
    
    public UserRole Role { get; set; } = UserRole.User;

    public int PointsBalance { get; set; } = 0;
    public decimal Rating { get; set; } = 5.0m;
    
    // Агрегаты эко-трекера
    public decimal SavedKgTotal { get; set; } = 0m;
    public decimal SavedCo2Total { get; set; } = 0m;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
    public ICollection<ExchangePoint> ManagedPoints { get; set; } = new List<ExchangePoint>();
    public ICollection<Transfer> GivenTransfers { get; set; } = new List<Transfer>();
    public ICollection<Transfer> ReceivedTransfers { get; set; } = new List<Transfer>();
}
