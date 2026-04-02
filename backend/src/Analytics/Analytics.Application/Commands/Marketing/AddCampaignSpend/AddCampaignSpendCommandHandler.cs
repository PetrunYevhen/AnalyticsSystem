using Analytics.Application.Auth;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using ValueObjects.ValueObject;

namespace Analytics.Application.Commands.Marketing.AddCampaignSpend;

public class AddCampaignSpendCommandHandler : IRequestHandler<AddCampaignSpendCommand, Result>
{
    private readonly ITenantContext _tenantContext;
    private readonly ICampaignRepository _campaignRepository;
    private readonly ILogger<AddCampaignSpendCommandHandler> _logger;

    public AddCampaignSpendCommandHandler(ITenantContext tenantContext, ICampaignRepository campaignRepository, ILogger<AddCampaignSpendCommandHandler> logger)
    {
        _tenantContext = tenantContext;
        _campaignRepository = campaignRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(AddCampaignSpendCommand request, CancellationToken ct)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        var campaign = await _campaignRepository.GetByIdAsync(tenantId, request.CampaignId, ct);

        if (campaign is null)
            return Result.Fail($"Кампанію не знайдено.");

        var currency = Currency.Parse(request.Currency);
        var spend = new Money(request.Amount, currency);

        try
        {
            campaign.AddSpend(spend);
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
        
        await _campaignRepository.UpdateAsync(campaign, ct);
        return Result.Ok();
    }
}