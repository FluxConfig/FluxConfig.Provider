using FluxConfig.Provider.Client.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FluxConfig.Provider;

internal sealed class FluxConfigurationProvider: ConfigurationProvider
{
    private readonly IFluxConfigClient _fluxConfigClient;
    private readonly TimeSpan _refreshInterval;
    private CancellationTokenSource _cts;
    private Task _configFetcherTask;

    internal FluxConfigurationProvider(IFluxConfigClient client, TimeSpan refreshInterval)
    {
        _fluxConfigClient = client;
        _refreshInterval = refreshInterval;
    }
}