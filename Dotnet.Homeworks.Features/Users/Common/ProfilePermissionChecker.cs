using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.Users.Common;

public class ProfilePermissionChecker : IPermissionChecker<IClientRequest>
{
    private readonly HttpContext? _httpContext;

    public ProfilePermissionChecker(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public Task<PermissionResult> ValidateAsync(IClientRequest request)
    {
        var claimGuid = _httpContext?.User.Claims
            .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(claimGuid, out var id))
            return Task.FromResult(new PermissionResult(false, "No id provided"));

        return Task.FromResult(id != request.Guid
            ? new PermissionResult(false, "Access denied")
            : new PermissionResult(true));
    }
}