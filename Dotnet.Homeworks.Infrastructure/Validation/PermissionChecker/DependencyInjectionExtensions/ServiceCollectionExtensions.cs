using System.Reflection;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;

public static class ServiceCollectionExtensions
{
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        Assembly assembly
    )
    {
        PermissionCheck.RegisterCheckersFromAssembly(assembly);
        serviceCollection.AddTransient<IPermissionCheck, PermissionCheck>();
    }
    
    public static void AddPermissionChecks(
        this IServiceCollection serviceCollection,
        Assembly[] assemblies
    )
    {
        PermissionCheck.RegisterCheckersFromAssembly(assemblies);
        serviceCollection.AddTransient<IPermissionCheck, PermissionCheck>();
    }
}