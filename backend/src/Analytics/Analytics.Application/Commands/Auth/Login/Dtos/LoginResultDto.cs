namespace Analytics.Application.Commands.Auth.Login.Dtos;

public record LoginResultDto(
    string JwtToken,
    Guid UserId,
    Guid TenantId,
    string FullName,
    string Email,
    string Role
);