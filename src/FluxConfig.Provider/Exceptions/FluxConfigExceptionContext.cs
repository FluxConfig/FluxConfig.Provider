using Microsoft.Extensions.Logging;

namespace FluxConfig.Provider.Exceptions;

public class FluxConfigExceptionContext
{
    public Exception? Exception { get; init; }
    public required ILogger<FluxConfigExceptionContext> Logger { get; set; }
    public required FluxConfigurationProvider Provider { get; set; }
}