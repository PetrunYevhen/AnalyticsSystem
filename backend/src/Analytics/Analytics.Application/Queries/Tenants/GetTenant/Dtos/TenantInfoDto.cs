namespace Analytics.Application.Queries.Tenants.GetTenant.Dtos;

public class TenantInfoDto
{
    public Guid Id { get; init; }
    public string CompanyName { get; init; } = default!;
    public string ApiKeyPrefix { get; init; } = default!;
}