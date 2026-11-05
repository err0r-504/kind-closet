using System.Security.Claims;
using kind_closet.Data;
using kind_closet.Models.DTOs;
using kind_closet.Models.Entities;
using kind_closet.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kind_closet.Controllers;

[ApiController]
[Route("api/v1/listings")]
public class ListingsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ListingCardDto>>> GetListings(
        [FromQuery] ListingFilterQuery filter,
        CancellationToken ct)
    {
        var query = db.Listings
            .AsNoTracking()
            .Include(l => l.Images)
            .Where(l => l.Status == ListingStatus.Published && l.ExpiresAt > DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
        {
            var q = filter.SearchQuery.Trim();
            query = query.Where(l => EF.Functions.ILike(l.Title, $"%{q}%") || EF.Functions.ILike(l.Description, $"%{q}%"));
        }

        if (filter.CategoryId.HasValue)
            query = query.Where(l => l.CategoryId == filter.CategoryId.Value);

        if (filter.PostType.HasValue)
            query = query.Where(l => l.PostType == filter.PostType.Value);

        if (filter.Condition.HasValue)
            query = query.Where(l => l.Condition == filter.Condition.Value);

        if (!string.IsNullOrWhiteSpace(filter.City))
            query = query.Where(l => l.City == filter.City);

        if (filter.ExchangeFormat.HasValue)
            query = query.Where(l => l.ExchangeFormat == filter.ExchangeFormat.Value);

        query = filter.SortBy == "popular"
            ? query.OrderByDescending(l => l.ViewsCount)
            : query.OrderByDescending(l => l.CreatedAt);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(l => new ListingCardDto(
                l.Id, l.Title, l.PostType, l.Condition, l.ExchangeFormat,
                l.PointsPrice, l.City, l.Status,
                l.Images.OrderBy(i => i.SortOrder).Select(i => i.ThumbnailObjectKey).FirstOrDefault(),
                l.CreatedAt))
            .ToListAsync(ct);

        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ListingDetailsDto>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var listing = await db.Listings
            .Include(l => l.Author)
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (listing is null) return NotFound();

        listing.ViewsCount++;
        await db.SaveChangesAsync(ct);

        return Ok(new ListingDetailsDto(
            listing.Id, listing.Title, listing.Description, listing.CategoryId,
            listing.PostType, listing.Condition, listing.Dimensions, listing.ExchangeFormat,
            listing.PointsPrice, listing.City, listing.Address, listing.Latitude, listing.Longitude,
            listing.ExchangePointId, listing.Status, listing.ViewsCount,
            listing.AuthorId, listing.Author.Name, listing.Author.Rating,
            listing.Images.OrderBy(i => i.SortOrder).Select(i => i.OriginalObjectKey).ToList(),
            listing.CreatedAt, listing.ExpiresAt));
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateListingRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var duration = request.DurationDays is > 0 and <= 90 ? request.DurationDays : 14;

        var listing = new Listing
        {
            AuthorId = userId,
            CategoryId = request.CategoryId,
            ExchangePointId = request.ExchangePointId,
            Title = request.Title,
            Description = request.Description,
            PostType = request.PostType,
            Condition = request.Condition,
            Dimensions = request.Dimensions,
            ExchangeFormat = request.ExchangeFormat,
            PointsPrice = request.PointsPrice,
            City = request.City,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = ListingStatus.Published,
            ExpiresAt = DateTime.UtcNow.AddDays(duration),
            Images = request.Images.Select((img, index) => new ListingImage
            {
                OriginalObjectKey = img.OriginalObjectKey,
                ThumbnailObjectKey = img.ThumbnailObjectKey,
                FileSize = img.FileSize,
                FileHash = img.FileHash,
                SortOrder = index
            }).ToList()
        };

        db.Listings.Add(listing);
        await db.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing.Id);
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateListingRequest request, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.AuthorId == userId, ct);
        if (listing is null) return NotFound();

        listing.Title = request.Title;
        listing.Description = request.Description;
        listing.CategoryId = request.CategoryId;
        listing.Condition = request.Condition;
        listing.Dimensions = request.Dimensions;
        listing.ExchangeFormat = request.ExchangeFormat;
        listing.PointsPrice = request.PointsPrice;
        listing.City = request.City;
        listing.Address = request.Address;

        await db.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id:guid}/republish")]
    public async Task<IActionResult> Republish([FromRoute] Guid id, [FromQuery] int durationDays = 14, CancellationToken ct = default)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.AuthorId == userId, ct);
        if (listing is null) return NotFound();

        listing.Status = ListingStatus.Published;
        listing.ExpiresAt = DateTime.UtcNow.AddDays(durationDays);
        await db.SaveChangesAsync(ct);

        return NoContent();
    }
}