using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public abstract class ValidationDecorator<TRequest, TResponse> : PermissionCheckDecorator<TRequest, TResponse>,
    IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    protected ValidationDecorator(
        IEnumerable<IValidator<TRequest>> validators,
        IPermissionCheck checker
    )
        : base(checker)
    {
        _validators = validators;
    }

    public new virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await base.Handle(request, cancellationToken);

        var error = _validators
            .Select(async validator => await validator.ValidateAsync(request, cancellationToken))
            .SelectMany(result => result.Result.Errors)
            .FirstOrDefault(failure => failure is not null)?.ErrorMessage;

        if (error is null)
            return await base.Handle(request, cancellationToken);

        return GenerateFailedResult(error);
    }
}