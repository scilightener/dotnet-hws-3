using Dotnet.Homeworks.Infrastructure.Utils;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

public interface IPermissionChecker<in TRequest>
{
    Task<PermissionResult> ValidateAsync(TRequest request);
}