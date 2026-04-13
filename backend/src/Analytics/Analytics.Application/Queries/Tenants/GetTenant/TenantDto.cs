namespace Analytics.Application.Queries.Tenants.GetTenant;

public record TenantDto(
    Guid Id, 
    string Name, 
    string Email, 
    DateTime CreatedAt);