using kind_closet.Models.Enums;

namespace kind_closet.Models.DTOs;

// Categories
public record CategoryDto(
    int Id, int? ParentId, string Name, string Slug,
    decimal DefaultWeightKg, decimal Co2PerKgFactor, List<CategoryDto> SubCategories);
