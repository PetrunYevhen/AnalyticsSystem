using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Marketing.DeleteFromCampaign;

public class DeleteCustomersFromCampaignCommand : CommandBase<Result>
{
    public Guid CampaignId { get; init; }
    public required List<Guid> CustomerIds { get; init; }
}