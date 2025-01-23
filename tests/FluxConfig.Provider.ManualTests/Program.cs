using FluxConfig.Provider.ManualTests.Options;
using FluxConfig.Provider.Options;
using FluxConfig.Provider.Options.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            options.ConnectionOptions = new ConnectionOptions
            {
                Address = new Uri(fluxConnection.StorageUrl),
                ApiKey = fluxConnection.ApiKey
            };
            options.PollingOptions = new FluxPollingOptions
            {
                ExceptionBehavior = PollingExceptionBehavior.Throw,
                RefreshInterval = TimeSpan.FromSeconds(10)
            };
            options.ConfigurationTag = fluxConnection.ConfigurationTag;
            
            options.LoggerFactory = LoggerFactory.Create(optionsBuilder =>
            {
                optionsBuilder.AddConsole();
                optionsBuilder.SetMinimumLevel(LogLevel.Critical);
            });
        });
        
        // Example of termination due to exception handler
        // builder.Configuration.SetFluxConfigExceptionHandler(ctx =>
        // {
        //     ctx.Logger.LogWarning("Handler: Exception occured while fetching config data: {message}", ctx.Exception?.Message);
        //     Environment.Exit(-1);
        // });

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
                await Task.Delay(TimeSpan.FromSeconds(10));
            } while (true);
        });
    }
}