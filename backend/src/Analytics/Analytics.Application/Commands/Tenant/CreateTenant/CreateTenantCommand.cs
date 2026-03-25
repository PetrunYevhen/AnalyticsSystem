using Analytics.Application.Commands.Tenant.CreateTenant.Dtos;
using Analytics.Application.Contracts;

namespace Analytics.Application.Commands.Tenant.CreateTenant;

public class CreateTenantCommand : CommandBase<CreateTenantResultDto>
{
    public required string CompanyName { get; set; }
}