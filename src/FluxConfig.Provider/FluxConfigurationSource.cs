using FluxConfig.Provider.Client.Interfaces;
using FluxConfig.Provider.Options;
using Microsoft.Extensions.Configuration;

namespace FluxConfig.Provider;

internal sealed class FluxConfigurationSource: IConfigurationSource
{
    public IFluxConfigClient? FluxConfigClient { get; set; }
    public TimeSpan RefreshInterval { get; set; }
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new FluxConfigurationProvider(
            client: FluxConfigClient!,
            refreshInterval: RefreshInterval
        );
    }
}