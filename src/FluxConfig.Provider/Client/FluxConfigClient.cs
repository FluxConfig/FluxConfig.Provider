using FluxConfig.Provider.Client.Interfaces;
using FluxConfig.Provider.Exceptions;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.GrpcContracts.Client;
using FluxConfig.Provider.Options.Enums;
using Grpc.Core;
using Grpc.Net.Client;

namespace FluxConfig.Provider.Client;

internal sealed class FluxConfigClient : IFluxConfigClient
{
    private readonly GrpcChannel _channel;
    private readonly PollingExceptionBehavior _pollingExceptionBehavior;
    private  bool _initialRequest = true;
    private readonly string _configurationTag;

    internal FluxConfigClient(GrpcChannel channel, PollingExceptionBehavior exceptionBehavior, string configurationTag)
    {
        _channel = channel;
        _pollingExceptionBehavior = exceptionBehavior;
        _configurationTag = configurationTag;
    }

    public async Task<Dictionary<string, string?>> LoadRealTimeConfigAsync(CancellationToken cancellationToken)
    {
        try
        {
            var fetchedConfig = await LoadRealTimeConfigAsyncUnsafe(
                channel: _channel,
                configurationTag: _configurationTag,
                cancellationToken: cancellationToken);
            
            _initialRequest = false;
            
            return fetchedConfig;
        }
        // TODO: Add custom FluxConfig logger
        catch (RpcException exception)
        {
            var fluxException = exception.GenerateFluxConfigException();

            if (_initialRequest || _pollingExceptionBehavior == PollingExceptionBehavior.Throw)
            {
                Console.WriteLine($"Exception occured while fetching realtime config data: {fluxException.Message}");
                throw fluxException;
            }

            Console.WriteLine($"Exception occured while fetching realtime config data: {fluxException.Message}");
            return new Dictionary<string, string?>();
        }
        catch (Exception ex)
        {
            if (_initialRequest || _pollingExceptionBehavior == PollingExceptionBehavior.Throw)
            {
                Console.WriteLine($"Exception occured while fetching realtime config data: {ex.Message}");
                throw new FluxConfigException("", ex);
            }

            Console.WriteLine($"Exception occured while fetching realtime config data: {ex.Message}");
            return new Dictionary<string, string?>();
        }
    }

    private static async Task<Dictionary<string, string?>> LoadRealTimeConfigAsyncUnsafe(
        GrpcChannel channel,
        string configurationTag,
        CancellationToken cancellationToken)
    {
        var client = new Storage.StorageClient(channel);

        var response = await client.LoadRealTimeConfigAsync(
            request: new LoadConfigRequest()
            {
                ConfigurationTag = configurationTag
            },
            cancellationToken: cancellationToken
        );

        return response.ConfigurationData.ToDictionary(x => x.Key, x => x.Value ?? null);
    }

    public async Task<Dictionary<string, string?>> LoadVaultConfigAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await LoadVaultConfigAsyncUnsafe(
                channel: _channel,
                configurationTag: _configurationTag,
                cancellationToken: cancellationToken
            );
        }
        catch (RpcException exception)
        {
            var fluxException = exception.GenerateFluxConfigException();
            throw fluxException;
        }
    }

    private static async Task<Dictionary<string, string?>> LoadVaultConfigAsyncUnsafe(
        GrpcChannel channel,
        string configurationTag,
        CancellationToken cancellationToken)
    {
        var client = new Storage.StorageClient(channel);

        var response = await client.LoadVaultConfigAsync(
            request: new LoadConfigRequest()
            {
                ConfigurationTag = configurationTag
            },
            cancellationToken: cancellationToken
        );

        return response.ConfigurationData.ToDictionary(x => x.Key, x => x.Value ?? null);
    }

    public Dictionary<string, string?> LoadVaultConfig()
    {
        try
        {
            return LoadVaultConfigUnsafe(
                channel: _channel,
                configurationTag: _configurationTag
            );
        }
        catch (RpcException exception)
        {
            var fluxException = exception.GenerateFluxConfigException();
            throw fluxException;
        }
    }

    private static Dictionary<string, string?> LoadVaultConfigUnsafe(
        GrpcChannel channel,
        string configurationTag
    )
    {
        var client = new Storage.StorageClient(channel);

        var response = client.LoadVaultConfig(
            request: new LoadConfigRequest()
            {
                ConfigurationTag = configurationTag
            }
        );

        return response.ConfigurationData.ToDictionary(x => x.Key, x => x.Value ?? null);
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}