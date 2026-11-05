using kind_closet.Data;
using kind_closet.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kind_closet.Controllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories(CancellationToken ct)
    {
        var all = await db.Categories.AsNoTracking().ToListAsync(ct);
        
        List<CategoryDto> BuildTree(int? parentId) =>
            all.Where(c => c.ParentId == parentId)
                .Select(c => new CategoryDto(
                    c.Id, c.ParentId, c.Name, c.Slug,
                    c.DefaultWeightKg, c.Co2PerKgFactor, BuildTree(c.Id)))
                .ToList();

        return Ok(BuildTree(null));
    }
}