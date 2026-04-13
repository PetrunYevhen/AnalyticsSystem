using Analytics.Domain.RepositoryContracts;
using MediatR;

namespace Analytics.Application.Commands.Tenant.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepository;

    public CreateTenantCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var newTenant = new Domain.Entities.Tenant.Tenant(
            request.AdminFullName,
            request.CompanyName,
            request.Email,
            request.Password
        );
        
        await _tenantRepository.AddAsync(newTenant);
        
        return newTenant.Id;
    }
}