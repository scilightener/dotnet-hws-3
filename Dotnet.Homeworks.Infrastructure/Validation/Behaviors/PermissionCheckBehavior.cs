using Dotnet.Homeworks.Infrastructure.Validation.Common;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Behaviors;

public class PermissionCheckBehavior<TRequest, TResponse> : CqrsDecoratorBase<TResponse>,
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IAdminRequest
    where TResponse : Result
{
    private readonly IPermissionCheck _checker;

    public PermissionCheckBehavior(IPermissionCheck checker)
    {
        _checker = checker;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var error = (await _checker.CheckPermissionAsync(request)).ToList();

        if (error.All(x => x.IsSuccess))
            return await next();

        return GenerateFailedResult(error.FirstOrDefault()?.Error ?? "");
    }
}