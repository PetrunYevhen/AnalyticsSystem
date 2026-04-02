using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Marketing.AssignToCampaign;

public class AssignCampaignToCustomersCommand : CommandBase<Result>
{
    public Guid CampaignId { get; set; }
    public List<Guid>? CustomerIds { get; set; }
}