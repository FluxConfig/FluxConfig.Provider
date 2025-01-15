using System.Diagnostics;
using FluxConfig.Provider.Client.Interfaces;
using FluxConfig.Provider.GrpcContracts.Client;
using Grpc.Net.Client;

namespace FluxConfig.Provider.Client;

// TODO: Change dispose pattern
internal sealed class FluxConfigClient : IFluxConfigClient, IDisposable
{
    private readonly GrpcChannel _channel;

    private readonly string _configurationTag;

    internal FluxConfigClient(GrpcChannel channel, string configurationTag)
    {
        _channel = channel;
        _configurationTag = configurationTag;
    }

    public async Task<Dictionary<string, string>> LoadRealTimeConfigAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await LoadRealTimeConfigAsyncUnsafe(
                channel: _channel,
                configurationTag: _configurationTag,
                cancellationToken: cancellationToken);
        }
        //TODO: rework for exception handling
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception occured while fetching config data: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }

    private static async Task<Dictionary<string, string>> LoadRealTimeConfigAsyncUnsafe(
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

        return response.ConfigurationData.ToDictionary(x => x.Key, x => x.Value);
    }

    public async Task<Dictionary<string, string>> LoadVaultConfigAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await LoadVaultConfigAsyncUnsafe(
                channel: _channel,
                configurationTag: _configurationTag,
                cancellationToken: cancellationToken
            );
        }
        //TODO: rework for exception handling
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception occured while fetching config data: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }

    private static async Task<Dictionary<string, string>> LoadVaultConfigAsyncUnsafe(
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

        return response.ConfigurationData.ToDictionary(x => x.Key, x => x.Value);
    }

    public Dictionary<string, string> LoadVaultConfig()
    {
        try
        {
            return LoadVaultConfigUnsafe(
                channel: _channel,
                configurationTag: _configurationTag
            );
        }
        //TODO: rework for exception handling
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception occured while fetching config data: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }

    private static Dictionary<string, string> LoadVaultConfigUnsafe(
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

        return response.ConfigurationData.ToDictionary(x => x.Key, x => x.Value);
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}