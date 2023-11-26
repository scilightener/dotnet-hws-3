using Dotnet.Homeworks.MainProject.Configuration;
using MassTransit;

namespace Dotnet.Homeworks.MainProject.ServicesExtensions.Masstransit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        RabbitMqConfig rabbitConfiguration)
    {
        services.AddMassTransit(c =>
        {
            c.SetKebabCaseEndpointNameFormatter();
            c.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(rabbitConfiguration.Hostname, "/", rc =>
                {
                    rc.Username(rabbitConfiguration.Username);
                    rc.Password(rabbitConfiguration.Password);
                });
        
                cfg.ConfigureEndpoints(ctx);
            });
        });
        return services;
    }
}