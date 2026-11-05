using System.Security.Claims;
using kind_closet.Data;
using kind_closet.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kind_closet.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController(AppDbContext db) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return NotFound();

        return Ok(new UserProfileDto(
            user.Id, user.Name, user.Email, user.Phone, user.City,
            user.AvatarObjectKey, user.Role, user.PointsBalance,
            user.Rating, user.SavedKgTotal, user.SavedCo2Total));
    }

    [HttpPatch("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMe([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return NotFound();

        user.Name = request.Name;
        user.City = request.City;
        user.AvatarObjectKey = request.AvatarObjectKey;

        await db.SaveChangesAsync(ct);

        return Ok(new UserProfileDto(
            user.Id, user.Name, user.Email, user.Phone, user.City,
            user.AvatarObjectKey, user.Role, user.PointsBalance,
            user.Rating, user.SavedKgTotal, user.SavedCo2Total));
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
}