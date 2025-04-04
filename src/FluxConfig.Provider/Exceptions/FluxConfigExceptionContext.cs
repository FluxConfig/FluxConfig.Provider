using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FluxConfig.Provider.Exceptions;

/// <summary>
/// Defines exception context which is captured by FluxConfigExceptionHandler optionally set by <see cref="Microsoft.Extensions.Configuration.ConfigurationBuilderExtensions.SetFluxConfigExceptionHandler"/>
/// in <see cref="Microsoft.Extensions.Configuration.ConfigurationBuilderExtensions"/>
/// </summary>
public class FluxConfigExceptionContext
{
    /// <summary>
    /// Exception which occured during <see cref="FluxConfigurationProvider"/> execution
    /// </summary>
    public Exception? Exception { get; init; }
    
    /// <summary>
    /// Instance of <see cref="ILogger"/> created by optionally set <see cref="FluxConfig.Provider.Options.FluxConfigOptions.LoggerFactory"/> in
    /// <see cref="FluxConfig.Provider.Options.FluxConfigOptions"/>, if not set use <see cref="Microsoft.Extensions.Logging.Abstractions.NullLogger"/>
    /// </summary>
    public ILogger<FluxConfigExceptionContext> Logger { get; set; } = NullLogger<FluxConfigExceptionContext>.Instance;
    
    /// <summary>
    /// Instance of <see cref="FluxConfigurationProvider"/> which caused an exception
    /// </summary>
    public  FluxConfigurationProvider? Provider { get; set; }
}