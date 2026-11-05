using kind_closet.Data;
using kind_closet.Models.DTOs;
using kind_closet.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace kind_closet.Controllers;

[ApiController]
[Route("api/v1/exchange-points")]
public class ExchangePointsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ExchangePointMarkerDto>>> GetPoints(
        [FromQuery] ExchangePointFilterQuery filter,
        CancellationToken ct)
    {
        var query = db.ExchangePoints.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.City))
            query = query.Where(p => p.City == filter.City);

        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);

        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.AcceptedCategoryIds.Contains(filter.CategoryId.Value));

        if (filter.ExchangeFormat.HasValue)
            query = query.Where(p => p.SupportedFormats.Contains(filter.ExchangeFormat.Value));

        var points = await query
            .Select(p => new ExchangePointMarkerDto(
                p.Id, p.Title, p.City, p.Address, p.Latitude, p.Longitude,
                p.Status, p.IsPartner, p.SupportedFormats, p.AcceptedCategoryIds))
            .ToListAsync(ct);

        return Ok(points);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExchangePointDetailsDto>> GetPointById([FromRoute] Guid id, CancellationToken ct)
    {
        var point = await db.ExchangePoints
            .AsNoTracking()
            .Include(p => p.Listings.Where(l => l.Status == ListingStatus.Published))
            .ThenInclude(l => l.Images)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (point is null) return NotFound();

        var activeListings = point.Listings.Select(l => new ListingCardDto(
            l.Id, l.Title, l.PostType, l.Condition, l.ExchangeFormat,
            l.PointsPrice, l.City, l.Status,
            l.Images.OrderBy(i => i.SortOrder).Select(i => i.ThumbnailObjectKey).FirstOrDefault(),
            l.CreatedAt)).ToList();

        return Ok(new ExchangePointDetailsDto(
            point.Id, point.Title, point.City, point.Address,
            point.Latitude, point.Longitude, point.WorkingHoursJson, point.ContactsJson,
            point.SupportedFormats, point.AcceptedCategoryIds, point.Status,
            point.IsPartner, activeListings));
    }
}