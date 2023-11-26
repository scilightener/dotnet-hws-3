using Dotnet.Homeworks.Infrastructure.Validation.Behaviors;
using Dotnet.Homeworks.Infrastructure.Validation.PermissionChecker.DependencyInjectionExtensions;
using Dotnet.Homeworks.Mediator.DependencyInjectionExtensions;
using FluentValidation;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.CqrsValidation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCqrsValidationDependency(this IServiceCollection serviceCollection)
    {
        var featureAssembly = Features.Helpers.AssemblyReference.Assembly;

        serviceCollection
            .AddMediator(featureAssembly)
            .AddValidatorsFromAssembly(featureAssembly)
            .AddPermissionChecks(featureAssembly);

        serviceCollection
            .AddTransient(typeof(Mediator.IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .AddTransient(typeof(Mediator.IPipelineBehavior<,>), typeof(PermissionCheckBehavior<,>));

        return serviceCollection;
    }
}