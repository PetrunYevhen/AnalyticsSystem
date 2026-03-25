using Analytics.Application.Contracts;
using Analytics.Application.Queries.Tenants.GetTenant.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Tenants.GetTenant;

public class GetTenantQuery : QueryBase<Result<TenantInfoDto>>;

