using Dotnet.Homeworks.Infrastructure.Validation.Common;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Mediator;
using Dotnet.Homeworks.Shared.Dto;

namespace Dotnet.Homeworks.Infrastructure.Validation.Decorators;

public class PermissionCheckDecorator<TRequest, TResponse> : CqrsDecoratorBase<TResponse>,
    IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IPermissionCheck _checker;

    protected PermissionCheckDecorator(
        IPermissionCheck checker
    )
    {
        _checker = checker;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var error = (await _checker.CheckPermissionAsync(request)).ToList();

        return error.All(x => x.IsSuccess)
            ? GenerateSucceedResult()
            : GenerateFailedResult(error.FirstOrDefault(x => !x.IsSuccess)?.Error ?? "");
    }
}