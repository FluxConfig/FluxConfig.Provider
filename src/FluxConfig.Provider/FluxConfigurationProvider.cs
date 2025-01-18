using FluxConfig.Provider.Client.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FluxConfig.Provider;

internal sealed class FluxConfigurationProvider : ConfigurationProvider
{
    private readonly IFluxConfigClient _fluxConfigClient;
    private readonly TimeSpan _refreshInterval;
    private CancellationTokenSource? _cts = null;
    private Task? _configFetcherTask = null;

    internal FluxConfigurationProvider(IFluxConfigClient client, TimeSpan refreshInterval)
    {
        _fluxConfigClient = client;
        _refreshInterval = refreshInterval;
    }

    public override void Load()
    {
        LoadVaultConfig();
        LoadRealTimeConfig(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

        if (_cts != null)
        {
            return;
        }

        _cts = new CancellationTokenSource();
        var cancellationToken = _cts.Token;

        _configFetcherTask ??=
            Task.Run(async () => await FetchRealTimeConfigTask(cancellationToken), cancellationToken);
    }

    private async Task FetchRealTimeConfigTask(CancellationToken cancellationToken)
    {
        using PeriodicTimer periodicTimer = new PeriodicTimer(_refreshInterval);
        try
        {
            while (await periodicTimer.WaitForNextTickAsync(cancellationToken))
            {
                await LoadRealTimeConfig(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private async Task LoadRealTimeConfig(CancellationToken cancellationToken)
    {
        Dictionary<string, string?> currentData = await _fluxConfigClient.LoadRealTimeConfigAsync(cancellationToken);

        if (HasSameData(currentData))
        {
            return;
        }

        Data = currentData;
        OnReload();
    }

    private void LoadVaultConfig()
    {
        Dictionary<string, string?> currentData = _fluxConfigClient.LoadVaultConfig();

        if (HasSameData(currentData))
        {
            return;
        }

        Data = currentData;

        OnReload();
    }

    private bool HasSameData(Dictionary<string, string?> newData)
    {
        if (Data.Count != newData.Count)
        {
            return false;
        }

        foreach (var (key, newValue) in newData)
        {
            if (!Data.TryGetValue(key, out string? curValue) || curValue != newValue)
            {
                return false;
            }
        }

        return true;
    }
}