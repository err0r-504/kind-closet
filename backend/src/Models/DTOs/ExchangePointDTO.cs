using kind_closet.Models.Enums;

namespace kind_closet.Models.DTOs;

// Exchange Points
public record ExchangePointFilterQuery(string? City, int? CategoryId, ExchangeFormat? ExchangeFormat, PointStatus? Status);

public record ExchangePointMarkerDto(
    Guid Id, string Title, string City, string Address,
    double Latitude, double Longitude, PointStatus Status, bool IsPartner,
    ExchangeFormat[] SupportedFormats, int[] AcceptedCategoryIds);

public record ExchangePointDetailsDto(
    Guid Id, string Title, string City, string Address,
    double Latitude, double Longitude, string WorkingHoursJson, string ContactsJson,
    ExchangeFormat[] SupportedFormats, int[] AcceptedCategoryIds,
    PointStatus Status, bool IsPartner, List<ListingCardDto> ActiveListings);
