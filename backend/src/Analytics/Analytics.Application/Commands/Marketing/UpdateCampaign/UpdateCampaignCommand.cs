using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Marketing.UpdateCampaign;

public class UpdateCampaignCommand : CommandBase<Result>
{
    public Guid CampaignId { get; set; }
    public decimal? Budget { get; init; }
    public string? Currency { get; init; }
    public decimal? ActualSpend { get; init; }
    public string? Status { get; init; }
    public DateOnly? EndDate { get; init; }
}