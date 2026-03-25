namespace Analytics.Application.Queries.Dashboard.Dtos;

public record StatItemDto(
    decimal Value,
    double ChangePercent);