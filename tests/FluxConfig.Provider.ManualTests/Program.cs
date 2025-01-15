using FluxConfig.Provider.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FluxConfig.Provider.ManualTests;

public sealed class Program
{
    public static void Main()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            configurationBuilder.AddFluxConfig(options =>
            {
                options.ConnectionOptions = new ConnectionOptions(
                    address: new Uri("https://localhost:7045"),
                    apiKey: "TEST-API-KEY"
                );
                options.ConfigurationTag = "Development";
                options.RefreshInterval = TimeSpan.FromSeconds(20);
            });
        });
    }
}