namespace Analytics.Application.Queries.Users.GetUser.Dtos;

public record ProfileInfoDto(
    string FullName,
    string Email,
    string Role);