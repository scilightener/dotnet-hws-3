using System.Security.Claims;
using Dotnet.Homeworks.Infrastructure.Utils;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.Enums;
using Dotnet.Homeworks.Infrastructure.Validation.RequestTypes;
using Microsoft.AspNetCore.Http;

namespace Dotnet.Homeworks.Features.UserManagement.Common;

public class AdminPermissionChecker : IPermissionChecker<IAdminRequest>
{
    private readonly HttpContext? _httpContext;

    public AdminPermissionChecker(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public Task<PermissionResult> ValidateAsync(IAdminRequest request)
    {
        var role = _httpContext?.User.Claims
            .FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

        return Task.FromResult(role != Roles.Admin.ToString()
            ? new PermissionResult(false, "Access denied")
            : new PermissionResult(true));
    }
}