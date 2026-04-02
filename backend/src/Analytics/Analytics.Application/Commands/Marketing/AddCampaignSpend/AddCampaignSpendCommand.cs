using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Marketing.AddCampaignSpend;

public class AddCampaignSpendCommand : CommandBase<Result>
{
    public Guid CampaignId { get; set; }
    public  decimal Amount { get; set; }
    public string Currency { get; set; }
}