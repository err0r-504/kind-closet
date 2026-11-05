using kind_closet.Models.Enums;

namespace kind_closet.Models.DTOs;

// Users
public record UserProfileDto(
    Guid Id, string Name, string? Email, string? Phone, string City,
    string? AvatarUrl, UserRole Role, int PointsBalance, decimal Rating,
    decimal SavedKgTotal, decimal SavedCo2Total);
public record UpdateProfileRequest(string Name, string City, string? AvatarObjectKey);
