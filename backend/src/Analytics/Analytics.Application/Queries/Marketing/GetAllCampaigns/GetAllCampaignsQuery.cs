using Analytics.Application.Caching;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Marketing.GetAllCampaigns.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Marketing.GetAllCampaigns;

public class GetAllCampaignsQuery : QueryBase<Result<List<CampaignDto>>>, ICacheableQuery
{
    public string CacheKeyIdentifier => $"campaigns:get-all-campaigns";
    public TimeSpan CacheTtl => TimeSpan.FromMinutes(5);
}