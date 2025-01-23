using FluxConfig.Provider.Client;
using FluxConfig.Provider.Exceptions;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.Options;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FluxConfig.Provider;

internal sealed class FluxConfigurationSource : IConfigurationSource
{
    public FluxConfigOptions? ConfigOptions { get; set; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        ConfigureDefaultLoggerFactory(ConfigOptions);

        return new FluxConfigurationProvider(
            client: BuildConfigClient(ConfigOptions),
            loggerFactory: ConfigOptions?.LoggerFactory!,
            handler: BuildExceptionHandler(builder),
            refreshInterval: ConfigOptions!.PollingOptions!.RefreshInterval
        );
    }

    private static void ConfigureDefaultLoggerFactory(FluxConfigOptions? options)
    {
        ThrowExt.ThrowIfNull(options, nameof(options));

        if (options!.LoggerFactory is null)
        {
            options.LoggerFactory = NullLoggerFactory.Instance;
        }
    }

    private static Action<FluxConfigExceptionContext> BuildExceptionHandler(IConfigurationBuilder builder)
    {
        return builder.GetFluxConfigExceptionHandler() ?? (exceptionContext =>
        {
            exceptionContext.Logger.LogWarning("Handler: Exception occured while fetching config data: {message}",
                exceptionContext.Exception?.Message);
        });
    }

    private static FluxConfigClient BuildConfigClient(FluxConfigOptions? options)
    {
        ThrowExt.ThrowIfNull(options, nameof(options));
        ThrowExt.ThrowIfNull(options!.PollingOptions, nameof(options.PollingOptions));
        ThrowExt.ThrowIfNull(options.ConnectionOptions, nameof(options.ConnectionOptions));
        ThrowExt.ThrowIfNull(options.ConfigurationTag, nameof(options.ConfigurationTag));

        return new FluxConfigClient(
            channel: BuildGrpcChannel(options.ConnectionOptions!),
            logger: options.LoggerFactory!.CreateLogger<FluxConfigClient>(),
            exceptionBehavior: options.PollingOptions!.ExceptionBehavior,
            configurationTag: options.ConfigurationTag!
        );
    }

    private static GrpcChannel BuildGrpcChannel(ConnectionOptions options)
    {
        ThrowExt.ThrowIfNull(options.Address, nameof(options.Address));
        ThrowExt.ThrowIfNull(options.ApiKey, nameof(options.ApiKey));

        var credentials = CallCredentials.FromInterceptor((context, metadata) =>
        {
            metadata.Add(ConnectionOptions.ApiKeyHeader, options.ApiKey!);
            return Task.CompletedTask;
        });

        var channel = GrpcChannel.ForAddress(
            address: options.Address!,
            channelOptions: new GrpcChannelOptions
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });

        return channel;
    }
}