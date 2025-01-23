using FluxConfig.Provider.Client.Interfaces;
using FluxConfig.Provider.Exceptions;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.GrpcContracts.Client;
using FluxConfig.Provider.Logging;
using FluxConfig.Provider.Options.Enums;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace FluxConfig.Provider.Client;

internal sealed class FluxConfigClient : IFluxConfigClient
{
    private readonly GrpcChannel _channel;
    private readonly ILogger<FluxConfigClient> _logger;
    private readonly PollingExceptionBehavior _pollingExceptionBehavior;
    private bool _initialRequest = true;
    private readonly string _configurationTag;

    internal FluxConfigClient(
        GrpcChannel channel,
        ILogger<FluxConfigClient> logger,
        PollingExceptionBehavior exceptionBehavior,
        string configurationTag)
    {
        _channel = channel;
        _logger = logger;
        _pollingExceptionBehavior = exceptionBehavior;
        _configurationTag = configurationTag;
    }

    public async Task<Dictionary<string, string?>> LoadRealTimeConfigAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogConfigDataFetchStart(
                curTime: DateTime.Now,
                configType: "RealTime",
                storageAddress: _channel.Target
                );
            
            var fetchedConfig = await LoadRealTimeConfigAsyncUnsafe(
                channel: _channel,
                configurationTag: _configurationTag,
                cancellationToken: cancellationToken);

            _initialRequest = false;
            
            _logger.LogConfigDataFetchFinish(
                curTime: DateTime.Now,
                configType: "RealTime",
                storageAddress: _channel.Target
            );

            return fetchedConfig;
        }
        catch (RpcException exception)
        {
            FluxConfigException fluxException = exception.GenerateFluxConfigException();

            if (_initialRequest || _pollingExceptionBehavior == PollingExceptionBehavior.Throw)
            {
                throw fluxException;
            }
            
            _logger.LogException(
                configType: "RealTime",
                curTime: DateTime.Now,
                exceptionMessage: fluxException.Message + $"\n{fluxException.InnerException?.Message}"
                );
            return new Dictionary<string, string?>();
        }
        catch (Exception ex)
        {
            if (_initialRequest || _pollingExceptionBehavior == PollingExceptionBehavior.Throw)
            {
                throw;
            }
            
            _logger.LogException(
                configType: "RealTime",
                curTime: DateTime.Now,
                exceptionMessage: ex.Message
            );
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
            _logger.LogConfigDataFetchStart(
                curTime: DateTime.Now,
                configType: "Vault",
                storageAddress: _channel.Target
            );
            
            var fetchedConfig =  await LoadVaultConfigAsyncUnsafe(
                channel: _channel,
                configurationTag: _configurationTag,
                cancellationToken: cancellationToken
            );
            
            _logger.LogConfigDataFetchFinish(
                curTime: DateTime.Now,
                configType: "Vault",
                storageAddress: _channel.Target
            );

            return fetchedConfig;
        }
        catch (RpcException exception)
        {
            var fluxException = exception.GenerateFluxConfigException();
            
            _logger.LogException(
                configType: "Vault",
                curTime: DateTime.Now,
                exceptionMessage: fluxException.Message
            );
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
            _logger.LogConfigDataFetchStart(
                curTime: DateTime.Now,
                configType: "Vault",
                storageAddress: _channel.Target
            );
            
            var fetchedConfig = LoadVaultConfigUnsafe(
                channel: _channel,
                configurationTag: _configurationTag
            );
            
            _logger.LogConfigDataFetchFinish(
                curTime: DateTime.Now,
                configType: "Vault",
                storageAddress: _channel.Target
            );

            return fetchedConfig;
        }
        catch (RpcException exception)
        {
            var fluxException = exception.GenerateFluxConfigException();
            
            _logger.LogException(
                configType: "Vault",
                curTime: DateTime.Now,
                exceptionMessage: fluxException.Message
            );
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
        _channel.Dispose();
    }
}