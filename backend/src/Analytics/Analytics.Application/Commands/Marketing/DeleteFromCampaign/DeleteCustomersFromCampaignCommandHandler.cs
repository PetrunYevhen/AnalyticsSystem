using Analytics.Application.Auth;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Marketing.DeleteFromCampaign;

public class DeleteCustomersFromCampaignCommandHandler : IRequestHandler<DeleteCustomersFromCampaignCommand, Result>
{
    private readonly ITenantContext _tenantContext;
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomersFromCampaignCommandHandler(ITenantContext tenantContext, ICustomerRepository customerRepository)
    {
        _tenantContext = tenantContext;
        _customerRepository = customerRepository;
    }

    public async Task<Result> Handle(DeleteCustomersFromCampaignCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerIds is null || request.CustomerIds.Count == 0)
            return Result.Fail("Не вказано жодного клієнта.");

        var tenantId = _tenantContext.RequiredTenantId();

        var customers = await _customerRepository.GetByIdsAsync(tenantId, request.CustomerIds, cancellationToken);
        if (customers.Count == 0)
            return Result.Fail("Клієнтів не знайдено.");

        var notInCampaign = customers.Where(c => c.CampaignId != request.CampaignId).ToList();
        if (notInCampaign.Any())
            return Result.Fail($"{notInCampaign.Count} клієнтів не належать до цієї кампанії.");

        foreach (var customer in customers)
            customer.DeleteFromCampaign();
        
        return Result.Ok();
    }
}