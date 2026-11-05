using kind_closet.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace kind_closet.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ExchangePoint> ExchangePoints => Set<ExchangePoint>();
    public DbSet<Listing> Listings => Set<Listing>();
    public DbSet<ListingImage> ListingImages => Set<ListingImage>();
    public DbSet<Transfer> Transfers => Set<Transfer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Включение расширения pg_trgm для полнотекстового поиска в PostgreSQL
        modelBuilder.HasPostgresExtension("pg_trgm");

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Phone).IsUnique();
            e.HasIndex(u => new { u.OAuthProvider, u.OAuthId });
            e.Property(u => u.Rating).HasPrecision(3, 2);
            e.Property(u => u.SavedKgTotal).HasPrecision(10, 2);
            e.Property(u => u.SavedCo2Total).HasPrecision(10, 2);
        });

        // Category (самоссылающаяся иерархия)
        modelBuilder.Entity<Category>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasIndex(c => c.Slug).IsUnique();
            e.Property(c => c.DefaultWeightKg).HasPrecision(8, 3);
            e.Property(c => c.Co2PerKgFactor).HasPrecision(8, 3);

            e.HasOne(c => c.Parent)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ExchangePoint
        modelBuilder.Entity<ExchangePoint>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.City, p.Status });
            e.Property(p => p.WorkingHoursJson).HasColumnType("jsonb");
            e.Property(p => p.ContactsJson).HasColumnType("jsonb");

            e.HasOne(p => p.Organizer)
                .WithMany(u => u.ManagedPoints)
                .HasForeignKey(p => p.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Listing
        modelBuilder.Entity<Listing>(e =>
        {
            e.HasKey(l => l.Id);
            e.HasIndex(l => new { l.Status, l.City, l.CategoryId, l.ExchangeFormat });
            e.HasIndex(l => l.CreatedAt);

            e.HasOne(l => l.Author)
                .WithMany(u => u.Listings)
                .HasForeignKey(l => l.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(l => l.Category)
                .WithMany(c => c.Listings)
                .HasForeignKey(l => l.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(l => l.ExchangePoint)
                .WithMany(p => p.Listings)
                .HasForeignKey(l => l.ExchangePointId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ListingImage
        modelBuilder.Entity<ListingImage>(e =>
        {
            e.HasKey(img => img.Id);
            e.HasOne(img => img.Listing)
                .WithMany(l => l.Images)
                .HasForeignKey(img => img.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transfer
        modelBuilder.Entity<Transfer>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasIndex(t => new { t.ListingId, t.Status });
            e.Property(t => t.CalculatedWeightKg).HasPrecision(8, 3);
            e.Property(t => t.CalculatedCo2Kg).HasPrecision(8, 3);

            e.HasOne(t => t.Listing)
                .WithMany(l => l.Transfers)
                .HasForeignKey(t => t.ListingId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Giver)
                .WithMany(u => u.GivenTransfers)
                .HasForeignKey(t => t.GiverId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.Receiver)
                .WithMany(u => u.ReceivedTransfers)
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.ExchangePoint)
                .WithMany()
                .HasForeignKey(t => t.ExchangePointId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}