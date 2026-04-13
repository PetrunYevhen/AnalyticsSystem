using Analytics.Application.Contracts;

namespace Analytics.Application.Commands.Tenant.CreateTenant;

public class CreateTenantCommand : CommandBase<Guid>
{
    public string AdminFullName { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}