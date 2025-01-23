using Microsoft.Extensions.Logging;

namespace FluxConfig.Provider.Options;

public class FluxConfigOptions
{
    public ConnectionOptions? ConnectionOptions { get; set; }
    public FluxPollingOptions? PollingOptions { get; set; } = new();
    public string? ConfigurationTag { get; set; }
    public ILoggerFactory? LoggerFactory { get; set; }
}