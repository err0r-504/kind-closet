using kind_closet.Models.Enums;

namespace kind_closet.Models.DTOs;

// Auth
public record RegisterRequest(string Email, string? Phone, string Password, string Name, string City);
public record LoginRequest(string EmailOrPhone, string Password);
public record OAuthLoginRequest(string Code, string RedirectUri);
public record RefreshTokenRequest(string RefreshToken);
public record AuthTokensResponse(string AccessToken, string RefreshToken, DateTime ExpiresAt);