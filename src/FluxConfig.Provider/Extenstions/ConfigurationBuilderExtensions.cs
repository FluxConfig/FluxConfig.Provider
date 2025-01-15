using FluxConfig.Provider;
using FluxConfig.Provider.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddFluxConfig(
        this IConfigurationBuilder configurationBuilder,
        Action<FluxConfigOptions> configureOptions)
    {
        configurationBuilder.Add<FluxConfigurationSource>(source =>
        {
            
        });
        
        return configurationBuilder;
    }
}