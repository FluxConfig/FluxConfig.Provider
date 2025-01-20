using FluxConfig.Provider.Options.Enums;

namespace FluxConfig.Provider.Options;

public class FluxPollingOptions
{
    public PollingExceptionBehavior ExceptionBehavior { get; set; } = PollingExceptionBehavior.Ignore;
    
    private TimeSpan _refreshInterval = TimeSpan.FromMilliseconds(15_000);

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

    public FluxPollingOptions(PollingExceptionBehavior exceptionBehavior, TimeSpan refreshInterval)
    {
        RefreshInterval = refreshInterval;
        ExceptionBehavior = exceptionBehavior;
    }
    
    public FluxPollingOptions () {}
}