using Microsoft.Extensions.Configuration;

namespace FluxConfig.Provider;

internal sealed class FluxConfigurationSource: IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        throw new NotImplementedException();
    }
}