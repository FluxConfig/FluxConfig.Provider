using FluxConfig.Provider.Client;
using FluxConfig.Provider.Exceptions;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.Options;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace FluxConfig.Provider;

internal sealed class FluxConfigurationSource : IConfigurationSource
{
    public FluxConfigOptions? ConfigOptions { get; set; }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new FluxConfigurationProvider(
            client: BuildConfigClient(ConfigOptions),
            handler: BuildExceptionHandler(builder),
            refreshInterval: ConfigOptions!.PollingOptions!.RefreshInterval
        );
    }

    private static Action<FluxConfigExceptionContext> BuildExceptionHandler(IConfigurationBuilder builder)
    {
        return builder.GetFluxConfigExceptionHandler() ?? (exceptionContext =>
        {
            // TODO: Change, Add custom FluxConfig logger
            Console.WriteLine($"Handler: Exception occured while fetching config data: {exceptionContext.Exception?.Message}");
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