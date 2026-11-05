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
[Authorize]
public class TransfersController(AppDbContext db) : ControllerBase
{
    [HttpPost("api/v1/listings/{listingId:guid}/respond")]
    public async Task<ActionResult<TransferDto>> RespondToListing(
        [FromRoute] Guid listingId,
        [FromBody] RespondToListingRequest request,
        CancellationToken ct)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == listingId, ct);

        if (listing is null) return NotFound("Объявление не найдено.");
        if (listing.AuthorId == currentUserId) return BadRequest("Нельзя откликнуться на собственное объявление.");
        if (listing.Status != ListingStatus.Published) return Conflict("Объявление уже забронировано или закрыто.");

        listing.Status = ListingStatus.Reserved;

        var isGiving = listing.PostType is PostType.Give or PostType.Lend;
        var transfer = new Transfer
        {
            ListingId = listing.Id,
            GiverId = isGiving ? listing.AuthorId : currentUserId,
            ReceiverId = isGiving ? currentUserId : listing.AuthorId,
            ExchangePointId = request.ExchangePointId ?? listing.ExchangePointId,
            Status = TransferStatus.Agreed
        };

        db.Transfers.Add(transfer);
        await db.SaveChangesAsync(ct);

        return Ok(MapToDto(transfer));
    }

    [HttpPost("api/v1/transfers/{id:guid}/confirm")]
    public async Task<ActionResult<TransferDto>> ConfirmTransfer([FromRoute] Guid id, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var transfer = await db.Transfers
            .Include(t => t.Listing)
            .ThenInclude(l => l.Category)
            .Include(t => t.Giver)
            .Include(t => t.Receiver)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (transfer is null) return NotFound();
        if (transfer.Status is TransferStatus.Completed or TransferStatus.Cancelled)
            return BadRequest("Сделка уже завершена или отменена.");

        if (currentUserId == transfer.GiverId)
            transfer.GiverConfirmed = true;
        else if (currentUserId == transfer.ReceiverId)
            transfer.ReceiverConfirmed = true;
        else
            return Forbid();

        // Если подтвердила только одна сторона — переводим в статус Transferred
        if (transfer.GiverConfirmed ^ transfer.ReceiverConfirmed)
        {
            transfer.Status = TransferStatus.Transferred;
            transfer.Listing.Status = ListingStatus.Transferred;
        }
        // Если обе стороны подтвердили — завершаем сделку, начисляем баллы и эко-метрики
        else if (transfer.GiverConfirmed && transfer.ReceiverConfirmed)
        {
            transfer.Status = TransferStatus.Completed;
            transfer.Listing.Status = ListingStatus.Completed;
            transfer.CompletedAt = DateTime.UtcNow;

            var weightKg = transfer.Listing.Category.DefaultWeightKg;
            var co2Kg = weightKg * transfer.Listing.Category.Co2PerKgFactor;
            var points = transfer.Listing.ExchangeFormat == ExchangeFormat.Points
                ? transfer.Listing.PointsPrice
                : 10; // Базовый бонус за передачу вещи

            transfer.CalculatedWeightKg = weightKg;
            transfer.CalculatedCo2Kg = co2Kg;
            transfer.AwardedPoints = points;

            transfer.Giver.PointsBalance += points;
            transfer.Giver.SavedKgTotal += weightKg;
            transfer.Giver.SavedCo2Total += co2Kg;

            if (transfer.Listing.ExchangeFormat == ExchangeFormat.Points)
                transfer.Receiver.PointsBalance = Math.Max(0, transfer.Receiver.PointsBalance - points);
        }

        await db.SaveChangesAsync(ct);
        return Ok(MapToDto(transfer));
    }

    [HttpPost("api/v1/transfers/{id:guid}/cancel")]
    public async Task<ActionResult<TransferDto>> CancelTransfer([FromRoute] Guid id, CancellationToken ct)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var transfer = await db.Transfers
            .Include(t => t.Listing)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (transfer is null) return NotFound();
        if (currentUserId != transfer.GiverId && currentUserId != transfer.ReceiverId) return Forbid();
        if (transfer.Status == TransferStatus.Completed) return BadRequest("Завершенную передачу нельзя отменить.");

        transfer.Status = TransferStatus.Cancelled;
        transfer.Listing.Status = ListingStatus.Published; // Возвращаем объявление в каталог

        await db.SaveChangesAsync(ct);
        return Ok(MapToDto(transfer));
    }

    private static TransferDto MapToDto(Transfer t) => new(
        t.Id, t.ListingId, t.GiverId, t.ReceiverId, t.Status,
        t.GiverConfirmed, t.ReceiverConfirmed, t.AwardedPoints,
        t.CalculatedWeightKg, t.CalculatedCo2Kg, t.CreatedAt, t.CompletedAt);
}