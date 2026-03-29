using Analytics.Application.Auth;
using Analytics.Application.Queries.Transactions.GetOrderPayment.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Transactions.GetOrderPayment;

public class GetOrderPaymentQueryHandler : IRequestHandler<GetOrderPaymentQuery, Result<OrderPaymentDto>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetOrderPaymentQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<OrderPaymentDto>> Handle(GetOrderPaymentQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                            SELECT 
                               "Id" AS "PaymentId",
                               "OrderId",
                               "Status",
                               "TotalAmount" AS "TotalAmount",
                               "PaidAmount" AS "PaidAmount",
                               "PaidCurrency" AS "Currency"
                           FROM "Analytics"."Payments"
                           WHERE "TenantId" = @TenantId
                             AND "OrderId"  = @OrderId;
                           
                           SELECT 
                               pt."Id",
                               pt."Amount" AS "Amount",
                               pt."Method",
                               pt."PaidAt",
                               pt."Type",
                               pt."Note"
                           FROM "Analytics"."PaymentTransactions" pt
                           JOIN "Analytics"."Payments" p ON p."Id" = pt."PaymentId"
                           WHERE pt."TenantId" = @TenantId
                             AND p."OrderId" = @OrderId
                           ORDER BY pt."PaidAt" ASC;
                            
                           """;

        var parameters = new
        {
            TenantId = tenantId,
            OrderId = request.OrderId
        };

        var multiQuery = await connection.QueryMultipleAsync
        (new CommandDefinition(
            sql, parameters, cancellationToken: cancellationToken));

        var payment = await multiQuery.ReadSingleOrDefaultAsync<OrderPaymentDto>();
        if (payment is null)
            return Result.Fail("Платіж не знайдено");

        var transactions = (await multiQuery.ReadAsync<PaymentTransactionDto>()).ToList();
        
        return Result.Ok(new OrderPaymentDto
        {
            PaymentId       = payment.PaymentId,
            OrderId         = payment.OrderId,
            Status          = payment.Status,
            TotalAmount     = payment.TotalAmount,
            PaidAmount      = payment.PaidAmount,
            RemainingAmount = payment.TotalAmount - payment.PaidAmount,
            RefundedAmount  = transactions.Where(t => t.Type == "Refund").Sum(t => t.Amount),
            Currency        = payment.Currency,
            Transactions    = transactions.AsList()
        });
    }
}