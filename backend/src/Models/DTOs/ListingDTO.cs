using kind_closet.Models.Enums;

namespace kind_closet.Models.DTOs;

// Listings
public record ListingFilterQuery(
    string? SearchQuery, int? CategoryId, PostType? PostType,
    ItemCondition? Condition, string? City, ExchangeFormat? ExchangeFormat,
    string SortBy = "date", int Page = 1, int PageSize = 20);

public record CreateListingRequest(
    string Title, string Description, int CategoryId, PostType PostType,
    ItemCondition Condition, string? Dimensions, ExchangeFormat ExchangeFormat,
    int PointsPrice, string City, string? Address, double? Latitude, double? Longitude,
    Guid? ExchangePointId, int DurationDays, List<UploadedMediaDto> Images);

public record UpdateListingRequest(
    string Title, string Description, int CategoryId, ItemCondition Condition,
    string? Dimensions, ExchangeFormat ExchangeFormat, int PointsPrice, string City, string? Address);

public record ListingCardDto(
    Guid Id, string Title, PostType PostType, ItemCondition Condition,
    ExchangeFormat ExchangeFormat, int PointsPrice, string City,
    ListingStatus Status, string? MainThumbnailUrl, DateTime CreatedAt);

public record ListingDetailsDto(
    Guid Id, string Title, string Description, int CategoryId, PostType PostType,
    ItemCondition Condition, string? Dimensions, ExchangeFormat ExchangeFormat,
    int PointsPrice, string City, string? Address, double? Latitude, double? Longitude,
    Guid? ExchangePointId, ListingStatus Status, int ViewsCount,
    Guid AuthorId, string AuthorName, decimal AuthorRating,
    List<string> ImageUrls, DateTime CreatedAt, DateTime ExpiresAt);