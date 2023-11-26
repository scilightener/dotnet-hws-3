namespace Dotnet.Homeworks.Storage.API.Configuration;

public class MinioConfig
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Endpoint { get; set; }
    public required int Port { get; set; }
    public required bool WithSsl { get; set; }
}