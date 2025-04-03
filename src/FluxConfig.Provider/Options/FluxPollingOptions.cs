using FluxConfig.Provider.Options.Enums;

namespace FluxConfig.Provider.Options;

/// <summary>
/// Options used to configure configuration data fetching behaviour and polling exception behaviour
/// </summary>
public class FluxPollingOptions
{
    /// <summary>
    /// Exception behaviour during realtime configuration data fetching
    /// </summary>
    public PollingExceptionBehavior ExceptionBehavior { get; set; } = PollingExceptionBehavior.Ignore;
    
    private TimeSpan _refreshInterval = TimeSpan.FromMilliseconds(15_000);

    /// <summary>
    /// Interval of configuration data fetching from FluxConfig system in milliseconds. Minimum value - 1000 ms.
    /// </summary>
    public TimeSpan RefreshInterval
    {
        get => _refreshInterval;
        set
        {
            if (value.TotalMilliseconds >= 1_000)
            {
                _refreshInterval = value;
            }
        }
    }

    /// <summary>
    /// Creates a new instance of options object with specified parameters
    /// </summary>
    /// <param name="exceptionBehavior">Exception behaviour during realtime configuration data fetching</param>
    /// <param name="refreshInterval">Interval of configuration data fetching from FluxConfig system in milliseconds. Minimum value - 1000 ms.</param>
    public FluxPollingOptions(PollingExceptionBehavior exceptionBehavior, TimeSpan refreshInterval)
    {
        RefreshInterval = refreshInterval;
        ExceptionBehavior = exceptionBehavior;
    }
    
    /// <summary>
    /// Creates a new instance of options object
    /// </summary>
    public FluxPollingOptions () {}
}