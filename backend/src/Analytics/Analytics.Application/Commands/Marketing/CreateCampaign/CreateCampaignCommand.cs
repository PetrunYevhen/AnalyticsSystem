using Analytics.Application.Contracts;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Marketing.CreateCampaign;

public class CreateCampaignCommand : CommandBase<Result<Guid>>
{
    public string Name { get; set; } = default!;
    public string Channel { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal Budget { get; set; }
}