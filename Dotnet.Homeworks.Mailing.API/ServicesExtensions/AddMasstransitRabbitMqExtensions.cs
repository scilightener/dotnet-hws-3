using Dotnet.Homeworks.Mailing.API.Configuration;
using MassTransit;

namespace Dotnet.Homeworks.Mailing.API.ServicesExtensions;

public static class AddMasstransitRabbitMqExtensions
{
    public static IServiceCollection AddMasstransitRabbitMq(this IServiceCollection services,
        RabbitMqConfig rabbitConfiguration)
    {
        services.AddMassTransit(c =>
        {
            c.SetKebabCaseEndpointNameFormatter();
            var assembly = typeof(Program).Assembly;
            c.AddConsumers(assembly);
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