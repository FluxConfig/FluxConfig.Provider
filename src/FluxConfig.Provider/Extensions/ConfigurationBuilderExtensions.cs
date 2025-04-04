using FluxConfig.Provider;
using FluxConfig.Provider.Exceptions;
using FluxConfig.Provider.Extensions;
using FluxConfig.Provider.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Provides <see cref="IConfigurationBuilder"/> extension methods for adding FluxConfig Provider - <see cref="FluxConfigurationProvider"/> as part of
/// application <see cref="IConfiguration"/> and configuring it
/// </summary>
public static class ConfigurationBuilderExtensions
{
    private const string FluxExceptionHandlerTag = "FluxConfigExceptionHandler";

    /// <summary>
    /// Adds FluxConfig Provider - <see cref="FluxConfigurationProvider"/> as part of
    /// application <see cref="IConfiguration"/> and configuring it 
    /// </summary>
    /// <param name="configurationBuilder">Instance of <see cref="IConfigurationBuilder"/></param>
    /// <param name="configureAction"><see cref="FluxConfig.Provider.Options.FluxConfigOptions"/> configuration action delegate </param>
    /// <returns></returns>
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

    /// <summary>
    /// Adds <see cref="FluxConfigurationProvider"/> internal exceptions handler,
    /// if not configured manually uses simple exception logging with <see cref="FluxConfig.Provider.Logging.LoggerExtensions.LogDefaultHandlerException"/>
    /// </summary>
    /// <param name="configurationBuilder">Instance of <see cref="IConfigurationBuilder"/></param>
    /// <param name="exceptionHandler"><see cref="FluxConfigurationProvider"/> internal exception handling action delegate</param>
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