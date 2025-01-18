using FluxConfig.Provider;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddFluxConfig(
        this IConfigurationBuilder configurationBuilder,
        Action<FluxConfigOptions> configureAction)
    {
        ThrowExt.ThrowIfNull(configurationBuilder, nameof(configurationBuilder));
        ThrowExt.ThrowIfNull(configureAction, nameof(configureAction));

        configurationBuilder.Add<FluxConfigurationSource>(source =>
        {
            var options = new FluxConfigOptions();
            configureAction(options);

            source.ConfigOptions = options;
        });

        return configurationBuilder;
    }
}