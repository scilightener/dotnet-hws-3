using System.Collections.Concurrent;
using System.Reflection;
using Dotnet.Homeworks.Infrastructure.Utils;

namespace Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker;

public class PermissionCheck : IPermissionCheck
{
    private static ConcurrentDictionary<Type, Type> CheckersByCommandTypes { get; set; } = new();
    private readonly IServiceProvider _serviceProvider;

    public PermissionCheck(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static void RegisterCheckersFromAssembly(Assembly assembly)
    {
        var scanResults = Scan(assembly);

        RegisterCheckers(scanResults);
    }

    public static void RegisterCheckersFromAssembly(Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var scanResults = Scan(assembly);

            RegisterCheckers(scanResults);
        }
    }

    public Task<IEnumerable<PermissionResult>> CheckPermissionAsync<TRequest>(TRequest request)
    {
        var checkerTypes = new List<Type>();
        var inheritedTypes = typeof(TRequest).GetInterfaces().ToList();
        inheritedTypes.Add(typeof(TRequest));

        foreach (var type in inheritedTypes)
        {
            if (CheckersByCommandTypes.TryGetValue(type, out var checkerType))
                checkerTypes.Add(checkerType);
        }

        var checker = checkerTypes.Select(GetCheckerInstance)
            .Select(async x => await ((IPermissionChecker<TRequest>)x).ValidateAsync(request))
            .Select(x => x.Result);

        return Task.FromResult(checker);
    }

    private static IEnumerable<ScanResult> Scan(Assembly assembly)
    {
        var result = assembly.GetTypes()
            .Select(x => new ScanResult(
                x.GetInterfaces().FirstOrDefault(inter =>
                    inter.Name == typeof(IPermissionChecker<>).Name)?.GetGenericArguments()[0],
                x
            ))
            .Where(x => x.CommandType is not null);

        return result;
    }

    private static void RegisterCheckers(IEnumerable<ScanResult> scanResults)
    {
        foreach (var scanResult in scanResults)
            CheckersByCommandTypes.AddOrUpdate(scanResult.CommandType!, scanResult.CheckerType,
                (_, _) => scanResult.CheckerType);
    }

    private object GetCheckerInstance(Type handlerType)
    {
        var constructor = handlerType.GetConstructors().Single();
        var parameters = constructor.GetParameters();
        var parameterValues = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var dependencyType = parameters[i].ParameterType;
            var dependencyInstance = _serviceProvider.GetService(dependencyType);
            parameterValues[i] = dependencyInstance ??
                                 throw new InvalidOperationException(
                                     $"Dependency with required type {dependencyType} is not registered");
        }

        return constructor.Invoke(parameterValues);
    }
    
    private record ScanResult(Type? CommandType, Type CheckerType);
}
