using Analytics.Application.Auth;
using Analytics.Domain.Entities.Marketing;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Marketing.AssignToCampaign;

public class AssignCampaignToCustomersCommandHandler : IRequestHandler<AssignCampaignToCustomersCommand, Result>
{
    private readonly ITenantContext _tenantContext;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICampaignRepository _campaignRepository;

    public AssignCampaignToCustomersCommandHandler(ITenantContext tenantContext, ICustomerRepository customerRepository, ICampaignRepository campaignRepository)
    {
        _tenantContext = tenantContext;
        _customerRepository = customerRepository;
        _campaignRepository = campaignRepository;
    }

    public async Task<Result> Handle(AssignCampaignToCustomersCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerIds is null || request.CustomerIds.Count == 0)
            return Result.Fail("Не вказано жодного клієнта");

        var tenantId = _tenantContext.RequiredTenantId();

        var campaign = await _campaignRepository.GetByIdAsync(tenantId, request.CampaignId, cancellationToken);
        if (campaign is null)
            return Result.Fail("Кампанію не знайдено");

        if (campaign.Status == CampaignStatus.Completed)
            return Result.Fail("Неможливо призначити клієнтів до завершеної кампанії");

        var customers = await _customerRepository.GetByIdsAsync(tenantId, request.CustomerIds, cancellationToken);
        if (customers.Count == 0)
            return Result.Fail("Клієнтів не знайдено");

        foreach (var customer in customers)
            customer.AssignToCampaign(campaign.Id, campaign.Channel);

        await _customerRepository.UpdateRangeAsync(customers, cancellationToken);
        return Result.Ok();
    }
}