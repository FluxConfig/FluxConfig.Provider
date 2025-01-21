using FluxConfig.Provider.Client.Interfaces;
using FluxConfig.Provider.Exceptions;
using Microsoft.Extensions.Configuration;

namespace FluxConfig.Provider;

public sealed class FluxConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly IFluxConfigClient _fluxConfigClient;
    private readonly TimeSpan _refreshInterval;
    private readonly Action<FluxConfigExceptionContext> _exceptionHandler;
    private bool _disposed;
    private CancellationTokenSource? _cts;
    private Task? _configFetcherTask;

    internal FluxConfigurationProvider(IFluxConfigClient client, Action<FluxConfigExceptionContext> handler,
        TimeSpan refreshInterval)
    {
        _fluxConfigClient = client;
        _exceptionHandler = handler;
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
                    catch (OperationCanceledException)
                    {
                        // Expected exception during disposing
                    }
                    catch (Exception ex)
                    {
                        _exceptionHandler(new FluxConfigExceptionContext
                            {
                                Exception = ex,
                                Provider = this
                            }
                        );
                    }
                }

                _cts?.Dispose();
                _fluxConfigClient.Dispose();
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
            throw new FluxConfigException("",
                new ObjectDisposedException("FluxConfigurationProvider instance was disposed"));
        }

        LoadVaultConfig();
        LoadRealTimeConfig(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

        if (_cts != null)
        {
            return;
        }

        _cts = new CancellationTokenSource();
        var cancellationToken = _cts.Token;

        _configFetcherTask ??= FetchRealTimeConfigTask(cancellationToken);
    }

    private async Task FetchRealTimeConfigTask(CancellationToken cancellationToken)
    {
        do
        {
            await Task.Delay(_refreshInterval, cancellationToken);
            try
            {
                await LoadRealTimeConfig(cancellationToken);
            }
            catch (Exception ex)
            {
                _exceptionHandler(new FluxConfigExceptionContext
                    {
                        Exception = ex,
                        Provider = this
                    }
                );
            }
        } while (!cancellationToken.IsCancellationRequested);
    }

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