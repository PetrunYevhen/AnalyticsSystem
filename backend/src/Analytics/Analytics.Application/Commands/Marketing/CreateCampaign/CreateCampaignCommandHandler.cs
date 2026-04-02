using Analytics.Application.Auth;
using Analytics.Domain.Entities.Marketing;
using Analytics.Domain.Enums;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;
using ValueObjects.ValueObject;

namespace Analytics.Application.Commands.Marketing.CreateCampaign;

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<Guid>>
{
    private readonly ITenantContext _tenantContext;
    private readonly ICampaignRepository _campaignRepository;

    public CreateCampaignCommandHandler(ITenantContext tenantContext, ICampaignRepository campaignRepository)
    {
        _tenantContext = tenantContext;
        _campaignRepository = campaignRepository;
    }

    public async Task<Result<Guid>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        if (!Enum.TryParse<AcquisitionChannel>(request.Channel, out var channel))
            return Result.Fail($"Невалідний канал: {request.Channel}");
        
        var activePeriod = request.EndDate.HasValue
            ? DateRange.Between(request.StartDate, request.EndDate.Value)
            : DateRange.StartingAt(request.StartDate);
        var budget = new Money(request.Budget, Currency.Parse("UAH"));
        var campaign = Campaign.Create(tenantId, request.Name, channel, activePeriod, budget);

        await _campaignRepository.AddAsync(campaign, cancellationToken);

        return Result.Ok(campaign.Id);
    }
}