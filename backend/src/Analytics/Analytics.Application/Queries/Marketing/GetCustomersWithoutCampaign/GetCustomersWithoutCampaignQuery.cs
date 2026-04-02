using Analytics.Application.Contracts;
using Analytics.Application.Queries.Marketing.GetCustomersWithoutCampaign.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Marketing.GetCustomersWithoutCampaign;

public class GetCustomersWithoutCampaignQuery : QueryBase<Result<List<CampaignCustomerDto>>>
{
}