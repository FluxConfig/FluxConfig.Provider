using FluxConfig.Provider;
using FluxConfig.Provider.Client;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.Options;
using Grpc.Core;
using Grpc.Net.Client;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddFluxConfig(
        this IConfigurationBuilder configurationBuilder,
        Action<FluxConfigOptions> configureAction)
    {
        ThrowExt.ThrowIfNull(configurationBuilder, nameof(configurationBuilder));
        ThrowExt.ThrowIfNull(configureAction, nameof(configureAction));

        configurationBuilder.Add<FluxConfigurationSource>(source =>
        {
            var options = new FluxConfigOptions();
            configureAction(options);

            source.FluxConfigClient = BuildConfigClient(options);
            source.RefreshInterval = options.RefreshInterval;
        });

        return configurationBuilder;
    }

    private static FluxConfigClient BuildConfigClient(FluxConfigOptions options)
    {
        ThrowExt.ThrowIfNull(options, nameof(options));
        ThrowExt.ThrowIfNull(options.ConnectionOptions, nameof(options.ConnectionOptions));
        ThrowExt.ThrowIfNull(options.ConfigurationTag, nameof(options.ConfigurationTag));

        return new FluxConfigClient(
            channel: BuildGrpcChannel(options.ConnectionOptions!),
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
            channelOptions: new GrpcChannelOptions()
            {
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });

        return channel;
    }
}