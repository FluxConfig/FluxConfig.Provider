using FluxConfig.Provider.Client.Interfaces;
using FluxConfig.Provider.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FluxConfig.Provider;

internal sealed class FluxConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly IFluxConfigClient _fluxConfigClient;
    private readonly TimeSpan _refreshInterval;
    private bool _disposed;
    private CancellationTokenSource? _cts = null;
    private Task? _configFetcherTask = null;

    internal FluxConfigurationProvider(IFluxConfigClient client, TimeSpan refreshInterval)
    {
        _fluxConfigClient = client;
        _refreshInterval = refreshInterval;
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _cts?.Cancel();

                if (_configFetcherTask != null)
                {
                    try
                    {
                        _configFetcherTask.ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        // TODO: Add exception handler to provider, e.g logger
                    }
                }
            
                _cts?.Dispose();
                _fluxConfigClient?.Dispose();
            }

            _configFetcherTask = null;
            _cts = null;
            
            _disposed = true;
        }
    }

    ~FluxConfigurationProvider()
    {
        Dispose(false);
    }

    #endregion
    

    public override void Load()
    {
        if (_disposed)
        {
            throw new FluxConfigException("", new ObjectDisposedException("FluxConfigurationProvider instance was disposed"));
        }
        LoadVaultConfig();

        if (_cts != null)
        {
            return;
        }

        _cts = new CancellationTokenSource();
        var cancellationToken = _cts.Token;

        _configFetcherTask ??=
            Task.Run(async () => await FetchRealTimeConfigTask(cancellationToken), cancellationToken);
    }

    //TODO: rework for exception handling
    private async Task FetchRealTimeConfigTask(CancellationToken cancellationToken)
    {
        await LoadRealTimeConfig(cancellationToken);
        
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
            // Expected exception during disposing
        }
    }

    //TODO: rework for exception handling
    private async Task LoadRealTimeConfig(CancellationToken cancellationToken)
    {
        Dictionary<string, string?> currentData = await _fluxConfigClient.LoadRealTimeConfigAsync(cancellationToken);

        if (currentData.Count == 0 || HasSameData(currentData))
        {
            return;
        }

        Data = currentData;
        OnReload();
    }

    //TODO: rework for exception handling
    private void LoadVaultConfig()
    {
        Dictionary<string, string?> currentData = _fluxConfigClient.LoadVaultConfig();

        if (currentData.Count == 0 || HasSameData(currentData))
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