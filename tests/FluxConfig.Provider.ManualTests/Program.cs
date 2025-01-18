using FluxConfig.Provider.ManualTests.Options;
using FluxConfig.Provider.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FluxConfig.Provider.ManualTests;

public sealed class Program
{
    public static async Task Main()
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            EnvironmentName = Environments.Development
        });

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
            options.RefreshInterval = TimeSpan.FromSeconds(5);
        });

        builder.Services.Configure<SimpleOptions>(
            builder.Configuration.GetSection("SimpleOptions"));

        using var host = builder.Build();

        var testOptionsMonitor = host.Services.GetRequiredService<IOptionsMonitor<SimpleOptions>>();

        await Task.Run(async () =>
        {
            do
            {
                var options = testOptionsMonitor.CurrentValue;
                
                Console.WriteLine($"{options.StringConfig}\n{options.IntConfig}\n{options.BoolConfig}\n");
                await Task.Delay(TimeSpan.FromSeconds(5));
            } while (true);
        });
    }
}