using Analytics.Application.Contracts;
using Analytics.Domain.Entities;

namespace Analytics.Application.Commands.CreateTenant;

public class CreateTenantCommand : CommandBase<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}