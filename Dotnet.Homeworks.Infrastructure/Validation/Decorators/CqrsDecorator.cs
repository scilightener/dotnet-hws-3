using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public class CqrsDecorator<TRequest, TResponse> : ValidationDecorator<TRequest, TResponse>,
    IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    protected CqrsDecorator(
        IEnumerable<IValidator<TRequest>> validators,
        IPermissionCheck checker
    )
        : base(validators, checker)
    {
    }

    public new virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        => await base.Handle(request, cancellationToken);
}