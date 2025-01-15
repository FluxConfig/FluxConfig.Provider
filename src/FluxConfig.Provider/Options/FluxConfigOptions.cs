namespace FluxConfig.Provider.Options;

public class FluxConfigOptions
{
    public ConnectionOptions? ConnectionOptions { get; set; }
    public string? ConfigurationTag { get; set; }

    private TimeSpan _refreshInterval = TimeSpan.FromMilliseconds(15_000);

    public TimeSpan RefreshInterval
    {
        get => _refreshInterval;
        set
        {
            if (value.TotalMilliseconds >= 1000)
            {
                _refreshInterval = value;
            }
        }
    }
}