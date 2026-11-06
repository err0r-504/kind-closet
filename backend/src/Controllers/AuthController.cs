using kind_closet.Data;
using kind_closet.Models.DTOs;
using kind_closet.Models.Entities;
using kind_closet.Models.Enums;
using kind_closet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kind_closet.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(AppDbContext db, IJwtTokenService jwtService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthTokensResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        // Проверяем, свободен ли Email
        var emailExists = await db.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (emailExists)
            return Conflict("Пользователь с таким Email уже существует.");

        // Создаем пользователя
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            City = request.City,
            PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(request.Password) // Надежное хэширование
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        // Генерируем токены
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        // TODO: Сохранить refreshToken в Redis или таблицу RefreshTokens

        return Ok(new AuthTokensResponse(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(60)));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthTokensResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var input = request.EmailOrPhone.Trim().ToLower();
        var isEmail = input.Contains('@');

        User? user;
        if (isEmail)
        {
            user = await db.Users.FirstOrDefaultAsync(u => u.Email == input, ct);
        }
        else
        {
            // Нормализация телефона: оставляем только цифры (и опционально + в начале)
            var normalizedPhone = new string(input.Where(c => char.IsDigit(c) || c == '+').ToArray());
            user = await db.Users.FirstOrDefaultAsync(u => u.Phone == normalizedPhone, ct);
        }

        if (user is null || user.PasswordHash is null || !BCrypt.Net.BCrypt.EnhancedVerify(request.Password, user.PasswordHash))
            return Unauthorized("Неверный логин или пароль.");

        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();

        return Ok(new AuthTokensResponse(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(60)));
    }
    
    [HttpPost("oauth/{provider}")]
    public async Task<ActionResult<AuthTokensResponse>> OAuthLogin(
        [FromRoute] OAuthProvider provider,
        [FromBody] OAuthLoginRequest request,
        CancellationToken ct)
    {
        if (provider is not (OAuthProvider.Vk or OAuthProvider.Yandex))
            return BadRequest("Поддерживаются только провайдеры VK и Яндекс.");

        // TODO: Обмен кода на токен провайдера, поиск/создание User, выдача JWT
        return Ok(new AuthTokensResponse("jwt-access-token", "refresh-token-guid", DateTime.UtcNow.AddMinutes(15)));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthTokensResponse>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        // TODO: Валидация и ротация Refresh-токена в Redis
        return Ok(new AuthTokensResponse("new-jwt-access-token", "new-refresh-token", DateTime.UtcNow.AddMinutes(15)));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        // TODO: Удаление Refresh-токена из Redis для принудительного разлогина
        return NoContent();
    }
    
    
}