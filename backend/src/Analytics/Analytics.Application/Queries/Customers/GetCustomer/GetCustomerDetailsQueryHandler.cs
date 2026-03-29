using Analytics.Application.Auth;
using Analytics.Application.Common;
using Analytics.Application.Metrics.Retention;
using Analytics.Application.Metrics.UnitEconomics;
using Analytics.Application.Queries.Customers.GetCustomer.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Customers.GetCustomer;

public class GetCustomerDetailsQueryHandler : IRequestHandler<GetCustomerDetailsQuery, Result<CustomerDetailsDto>>
{
    private readonly ILtvCalculator _ltvCalculator;
    private readonly ITenantContext _tenantContext;
    private readonly IRetentionStatsProvider _retentionReader;

    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetCustomerDetailsQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory, ILtvCalculator ltvCalculator, IRetentionStatsProvider retentionReader)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
        _ltvCalculator = ltvCalculator;
        _retentionReader = retentionReader;
    }

    public async Task<Result<CustomerDetailsDto>> Handle(GetCustomerDetailsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        using var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = """
                           SELECT
                               c."Id",
                               c."FullName",
                               c."Email",
                               c."PhoneNumber",
                               c."ExternalId",
                               c."Status",
                               c."RegistrationDate",
                               c."FirstOrderDate",
                               c."LastOrderDate",
                               c."OrderCount",
                               c."TotalRevenue",
                               c."AcquisitionChannel",
                               c."CampaignId",
                               camp."Name" AS "CampaignName"
                           FROM "Analytics"."Customers" c
                           LEFT JOIN "Analytics"."Campaigns" camp ON camp."Id" = c."CampaignId"
                           WHERE c."TenantId" = @TenantId
                            AND c."Id" = @CustomerId;

                           SELECT
                               o."Id" AS "OrderId",
                               o."ExternalOrderId",
                               o."OrderDate",
                               o."Status",
                               o."TotalAmount"
                           FROM "Analytics"."Orders" o
                           WHERE o."TenantId" = @TenantId
                            AND o."CustomerId" = @CustomerId
                           ORDER BY o."OrderDate" DESC
                           LIMIT 5;

                           SELECT COALESCE(SUM(oi."UnitCost" * oi."Quantity"), 0)
                           FROM "Analytics"."Orders" o
                           JOIN "Analytics"."OrderItem" oi ON oi."OrderId" = o."Id"
                           WHERE o."TenantId" = @TenantId
                             AND o."CustomerId" = @CustomerId;
                           """;

        var parameters = new
        {
            TenantId = tenantId,
            CustomerId = request.CustomerId
        };

        var multiQuery = await connection.QueryMultipleAsync(sql, parameters);
        var customer = await multiQuery.ReadFirstOrDefaultAsync<CustomerDetailsDto>();

        if (customer is null)
            return Result.Fail("Клієнта не знайдено.");

        var orders = (await multiQuery.ReadAsync<CustomerOrderDto>()).ToList();
        var totalCogs = await multiQuery.ReadFirstOrDefaultAsync<decimal>();

        LtvResult ltv = LtvResult.Empty;
        if (customer.FirstOrderDate is not null)
        {
            var grossMargin = customer.TotalRevenue > 0
                ? (customer.TotalRevenue - totalCogs) / customer.TotalRevenue
                : 0m;

            var cohortPeriod = Period.ForMonth(customer.FirstOrderDate.Value);
            var retention = await _retentionReader.GetAsync(tenantId, cohortPeriod, cancellationToken);

            ltv = _ltvCalculator.CalculateForCustomer(
                customerTotalRevenue: customer.TotalRevenue,
                firstOrderDate: DateOnly.FromDateTime(customer.FirstOrderDate.Value),
                calculationDate: DateOnly.FromDateTime(DateTime.UtcNow),
                cohortMonthlyChurnRate: retention.Value.ChurnRate,
                grossMargin: grossMargin);
        }     
        return Result.Ok(new CustomerDetailsDto
        {
            Id                 = customer.Id,
            FullName           = customer.FullName,
            Email              = customer.Email,
            PhoneNumber        = customer.PhoneNumber,
            ExternalId         = customer.ExternalId,
            Status             = customer.Status,
            RegistrationDate   = customer.RegistrationDate,
            FirstOrderDate     = customer.FirstOrderDate,
            LastOrderDate      = customer.LastOrderDate,
            OrderCount         = customer.OrderCount,
            TotalRevenue       = customer.TotalRevenue,
            PredictedLtv       = ltv.LifetimeValue,          
            ArpuMonthly        = ltv.ArpuMonthly,  
            AcquisitionChannel = customer.AcquisitionChannel,
            CampaignId         = customer.CampaignId,
            CampaignName       = customer.CampaignName,
            RecentOrders       = orders
        });    
    }
}