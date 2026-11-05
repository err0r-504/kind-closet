using System.ComponentModel.DataAnnotations;
using kind_closet.Models.Enums;

namespace kind_closet.Models.Entities;

public class Category
{
    public int Id { get; set; }
    public int? ParentId { get; set; }
    
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required, MaxLength(100)]
    public string Slug { get; set; } = string.Empty;

    // Коэффициенты для формулы эко-трекера по категории
    public decimal DefaultWeightKg { get; set; }
    public decimal Co2PerKgFactor { get; set; }

    public Category? Parent { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
