using Analytics.Application.Auth;
using Analytics.Application.Auth.ApiKey;
using Analytics.Application.Commands.Tenant.CreateTenant.Dtos;
using Analytics.Domain.RepositoryContracts;
using MediatR;

namespace Analytics.Application.Commands.Tenant.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, CreateTenantResultDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IApiKeyService _apiKeyService;

    public CreateTenantCommandHandler(ITenantRepository tenantRepository, IApiKeyService apiKeyService)
    {
        _tenantRepository = tenantRepository;
        _apiKeyService = apiKeyService;
    }

    public async Task<CreateTenantResultDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var (tenant, plainApiKey) = _apiKeyService.Create(request.CompanyName);
        
        await _tenantRepository.AddAsync(tenant);

        return new CreateTenantResultDto(
            TenantId: tenant.Id,
            ApiKey: plainApiKey,
            Message: "Збережіть цей API ключ. Він більше не буде показаний.");

    }
}