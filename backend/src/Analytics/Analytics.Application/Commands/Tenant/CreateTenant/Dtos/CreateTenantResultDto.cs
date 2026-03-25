namespace Analytics.Application.Commands.Tenant.CreateTenant.Dtos;

public record CreateTenantResultDto(
    Guid TenantId,
    string ApiKey,
    string Message);