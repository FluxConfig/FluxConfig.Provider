using FluxConfig.Provider;
using FluxConfig.Provider.Exceptions;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    private const string FluxExceptionHandlerTag = "FluxConfigExceptionHandler";

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

    public static void SetFluxConfigExceptionHandler(
        this IConfigurationBuilder configurationBuilder,
        Action<FluxConfigExceptionContext> exceptionHandler
    )
    {
        ThrowExt.ThrowIfNull(configurationBuilder, nameof(configurationBuilder));
        ThrowExt.ThrowIfNull(exceptionHandler, nameof(exceptionHandler));

        configurationBuilder.Properties.Add(FluxExceptionHandlerTag, exceptionHandler);
    }

    internal static Action<FluxConfigExceptionContext>? GetFluxConfigExceptionHandler(
        this IConfigurationBuilder configurationBuilder)
    {
        ThrowExt.ThrowIfNull(configurationBuilder, nameof(configurationBuilder));

        return configurationBuilder.Properties.TryGetValue(FluxExceptionHandlerTag, out var handler)
            ? handler as Action<FluxConfigExceptionContext>
            : null;
    }
}