using Dotnet.Homeworks.Storage.API.Configuration;
using Minio;

namespace Dotnet.Homeworks.Storage.API.ServicesExtensions;

public static class AddMinioExtensions
{
    public static IServiceCollection AddMinioClient(this IServiceCollection services,
        MinioConfig minioConfiguration)
    {
        var minioClient = new MinioClient()
            .WithEndpoint(minioConfiguration.Endpoint, minioConfiguration.Port)
            .WithCredentials(minioConfiguration.Username, minioConfiguration.Password)
            .WithSSL(minioConfiguration.WithSsl)
            .Build();
        services.AddSingleton(minioClient);
        return services;
    }
}