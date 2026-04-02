using Analytics.Application.Auth;
using Analytics.Domain.Entities.Marketing;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;
using ValueObjects.ValueObject;

namespace Analytics.Application.Commands.Marketing.UpdateCampaign;

public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, Result>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ITenantContext _tenantContext;

    public UpdateCampaignCommandHandler(ICampaignRepository campaignRepository, ITenantContext tenantContext)
    {
        _campaignRepository = campaignRepository;
        _tenantContext = tenantContext;
    }

    public async Task<Result> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        var campaign = await _campaignRepository.GetByIdAsync(tenantId, request.CampaignId, cancellationToken);
        if (campaign is null)
            return Result.Fail("Кампанію не знайдено");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);        
        if(request.Budget.HasValue)
        {
            var currency = Currency.Parse(request.Currency ?? "UAH");
            campaign.UpdateBudget(new Money(request.Budget.Value, currency));
        }
        if (request.EndDate.HasValue)
            campaign.UpdateEndDate(request.EndDate.Value, today);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<CampaignStatus>(request.Status, ignoreCase: true, out var status))
                return Result.Fail($"Невідомий статус: {request.Status}");

            var statusResult = status switch
            {
                CampaignStatus.Active when campaign.Status == CampaignStatus.Paused => campaign.Resume(today),
                CampaignStatus.Active    => campaign.Activate(today),
                CampaignStatus.Completed => campaign.Complete(),
                CampaignStatus.Paused    => campaign.Pause(),
                CampaignStatus.Draft     => Result.Fail("Не можна повернути в Draft."),
                _ => Result.Fail($"Невідомий статус: {status}")
            };

            if (statusResult.IsFailed)
                return statusResult;
        }

        await _campaignRepository.UpdateAsync(campaign, cancellationToken);
        return Result.Ok();
    }
}