using kind_closet.Models.Entities;

namespace kind_closet.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}