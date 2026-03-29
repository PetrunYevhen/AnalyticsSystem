using Analytics.Application.Commands.Orders.CreateOrderManually.Dto;
using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Orders.CreateOrderManually;

public class CreateOrderManuallyCommand : CommandBase<Result<CreateOrderResult>>
{
    public OrderDto? Order { get; init; }
    public CustomerDto? Customer { get; init; }
}