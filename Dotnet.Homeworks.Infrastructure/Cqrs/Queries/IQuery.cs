using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Cqrs.Queries;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}