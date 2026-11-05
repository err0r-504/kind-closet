using System.ComponentModel.DataAnnotations;
using kind_closet.Models.Enums;

namespace kind_closet.Models.Entities;

public class Transfer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ListingId { get; set; }
    public Guid GiverId { get; set; }
    public Guid ReceiverId { get; set; }
    public Guid? ExchangePointId { get; set; }

    public TransferStatus Status { get; set; } = TransferStatus.Agreed;
    
    // Двустороннее подтверждение передачи
    public bool GiverConfirmed { get; set; } = false;
    public bool ReceiverConfirmed { get; set; } = false;

    public int AwardedPoints { get; set; } = 0;
    public decimal CalculatedWeightKg { get; set; } = 0m;
    public decimal CalculatedCo2Kg { get; set; } = 0m;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public Listing Listing { get; set; } = null!;
    public User Giver { get; set; } = null!;
    public User Receiver { get; set; } = null!;
    public ExchangePoint? ExchangePoint { get; set; }
}