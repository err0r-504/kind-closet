using kind_closet.Models.DTOs;
using kind_closet.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace kind_closet.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthTokensResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        // TODO: Хэширование пароля, создание User в PostgreSQL, сохранение Refresh-токена в Redis
        return Ok(new AuthTokensResponse("jwt-access-token", "refresh-token-guid", DateTime.UtcNow.AddMinutes(15)));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthTokensResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        // TODO: Проверка учетных данных, выпуск JWT Access + сохранение Refresh в Redis
        return Ok(new AuthTokensResponse("jwt-access-token", "refresh-token-guid", DateTime.UtcNow.AddMinutes(15)));
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