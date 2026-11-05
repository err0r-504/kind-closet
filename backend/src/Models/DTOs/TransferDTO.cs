using kind_closet.Models.Enums;

namespace kind_closet.Models.DTOs;

// Transfers
public record RespondToListingRequest(Guid? ExchangePointId, string? InitialMessage);
public record TransferDto(
    Guid Id, Guid ListingId, Guid GiverId, Guid ReceiverId,
    TransferStatus Status, bool GiverConfirmed, bool ReceiverConfirmed,
    int AwardedPoints, decimal CalculatedWeightKg, decimal CalculatedCo2Kg,
    DateTime CreatedAt, DateTime? CompletedAt);