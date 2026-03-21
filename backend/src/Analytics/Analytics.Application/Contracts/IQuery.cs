using MediatR;

namespace Analytics.Application.Contracts;

public interface IQuery<out TResult> : IRequest<TResult>
{
}