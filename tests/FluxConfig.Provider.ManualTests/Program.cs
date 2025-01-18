using FluxConfig.Provider.ManualTests.Options;
using FluxConfig.Provider.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FluxConfig.Provider.ManualTests;

public sealed class Program
{
    public static void Main()
    {
        var builder = Host.CreateApplicationBuilder();

        TestConnectionOptions fluxConnection = builder.Configuration
                                                   .GetSection($"FluxConfigOptions:{nameof(TestConnectionOptions)}")
                                                   .Get<TestConnectionOptions>() ??
                                               throw new ArgumentException("FluxConfig connection options are missing.");

        builder.Configuration.AddFluxConfig(options =>
        {
            options.ConnectionOptions = new ConnectionOptions(
                address: new Uri(fluxConnection.StorageUrl),
                apiKey: fluxConnection.ApiKey
            );
            options.ConfigurationTag = fluxConnection.ConfigurationTag;
            options.RefreshInterval = TimeSpan.FromSeconds(20);
        });

        builder.Services.Configure<SimpleOptions>(
            builder.Configuration.GetSection($"FluxConfigOptions:{nameof(ConnectionOptions)}"));

        using var host = builder.Build();
    }
}