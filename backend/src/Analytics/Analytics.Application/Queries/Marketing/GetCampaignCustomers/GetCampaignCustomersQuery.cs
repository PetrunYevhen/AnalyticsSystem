using Analytics.Application.Caching;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Marketing.GetCampaignCustomers.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Marketing.GetCampaignCustomers;

public class GetCampaignCustomersQuery : QueryBase<Result<List<CampaignCustomerDto>>>
, ICacheableQuery
{
    public GetCampaignCustomersQuery(Guid campaignId)
    {
        CampaignId = campaignId;
    }

    public Guid CampaignId { get; init; }

    public string CacheKeyIdentifier => "campaigns:get-campaign-customers";
    public TimeSpan CacheTtl => TimeSpan.FromMinutes(5);
}