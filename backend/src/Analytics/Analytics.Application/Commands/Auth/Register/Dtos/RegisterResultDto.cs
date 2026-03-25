namespace Analytics.Application.Commands.Auth.Register.Dtos;

public record RegisterResultDto(
    Guid TenantId,
    Guid UserId,
    string JwtToken,
    string ApiKey,
    string Message
);