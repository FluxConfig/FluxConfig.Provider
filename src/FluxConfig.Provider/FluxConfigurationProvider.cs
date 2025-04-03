using FluxConfig.Provider.Client.Interfaces;
using FluxConfig.Provider.Exceptions;
using FluxConfig.Provider.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FluxConfig.Provider;

/// <summary>
/// FluxConfig System <see cref="ConfigurationProvider"/>
/// </summary>
public sealed class FluxConfigurationProvider : ConfigurationProvider, IDisposable
{
    private readonly IFluxConfigClient _fluxConfigClient;
    private readonly TimeSpan _refreshInterval;
    private readonly Action<FluxConfigExceptionContext> _exceptionHandler;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<FluxConfigurationProvider> _logger;
    private bool _disposed;
    private CancellationTokenSource? _cts;
    private Task? _configFetcherTask;
    
    internal FluxConfigurationProvider(
        IFluxConfigClient client,
        ILoggerFactory loggerFactory,
        Action<FluxConfigExceptionContext> handler,
        TimeSpan refreshInterval)
    {
        _fluxConfigClient = client;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<FluxConfigurationProvider>();
        _exceptionHandler = handler;
        _refreshInterval = refreshInterval;
    }

    #region IDisposable

    /// <summary>
    /// Release of unmanaged resources used by <see cref="FluxConfigurationProvider"/> in a thread-safe manner
    /// </summary>
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
                                Logger = _loggerFactory.CreateLogger<FluxConfigExceptionContext>(),
                                Provider = this
                            }
                        );
                    }
                }

                _cts?.Dispose();
                _fluxConfigClient.Dispose();
                _loggerFactory.Dispose();
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

    /// <summary>
    /// Loads the configuration data from FluxConfig.Storage service via <see cref="Client.FluxConfigClient"/>
    /// </summary>
    public override void Load()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _logger.StartProviderExecution( _fluxConfigClient.Address);

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
                        Logger = _loggerFactory.CreateLogger<FluxConfigExceptionContext>(),
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