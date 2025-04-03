using Microsoft.Extensions.Logging;

namespace FluxConfig.Provider.Options;

/// <summary>
/// Options used to configure <see cref="FluxConfigurationProvider"/>
/// </summary>
public class FluxConfigOptions
{
    /// <summary>
    /// Connection options used to connect to FluxConfig.Storage service
    /// </summary>
    public ConnectionOptions? ConnectionOptions { get; set; }
    
    /// <summary>
    /// Options used to configure configuration data fetching behaviour and polling exception behaviour
    /// </summary>
    public FluxPollingOptions? PollingOptions { get; set; } = new();
    
    /// <summary>
    /// Configuration tag used to claim corresponding service configuration data from FluxConfig system
    /// </summary>
    public string? ConfigurationTag { get; set; }
    
    /// <summary>
    /// Optional <see cref="ILoggerFactory"/> for
    /// <see cref="FluxConfigurationProvider"/> and its dependencies.
    /// If not specified  <see cref="Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory"/> is used.
    /// </summary>
    public ILoggerFactory? LoggerFactory { get; set; }
}