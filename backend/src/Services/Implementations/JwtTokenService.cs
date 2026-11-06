using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using kind_closet.Models.Entities;
using kind_closet.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace kind_closet.Services.Implementations;

public class JwtTokenService(IConfiguration config) : IJwtTokenService
{
    public string GenerateAccessToken(User user)
    {
        var jwtSettings = config.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"]!;
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]!);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N"); // Простой уникальный токен
    }
}