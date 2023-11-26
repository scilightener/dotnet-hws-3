using Dotnet.Homeworks.Infrastructure.Validation.Common;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;
using FluentValidation;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : CqrsDecoratorBase<TResponse>,
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IAdminRequest
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!_validators.Any())
                return await next();

            var error = _validators
                .Select(async validator => await validator.ValidateAsync(request, cancellationToken))
                .SelectMany(result => result.Result.Errors)
                .FirstOrDefault(failure => failure is not null)?.ErrorMessage;

            if (error is null)
                return await next();

            return GenerateFailedResult(error);
        }
        catch (Exception e)
        {
            return GenerateFailedResult(e.Message);
        }
    }
}