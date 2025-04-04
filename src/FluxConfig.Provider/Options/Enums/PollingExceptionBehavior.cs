namespace FluxConfig.Provider.Options.Enums;

/// <summary>
/// Configures exception behaviour during realtime configuration data fetching
/// </summary>
public enum PollingExceptionBehavior
{
    /// <summary>
    /// All exceptions thrown during realtime configuration data fetching are ignored 
    /// </summary>
    Ignore,
    
    /// <summary>
    /// All exceptions thrown during realtime configuration data fetching are handled
    /// via <see cref="FluxConfigurationProvider._exceptionHandler"/> configured by <see cref="Microsoft.Extensions.Configuration.ConfigurationBuilderExtensions.SetFluxConfigExceptionHandler"/>
    /// </summary>
    Throw
}