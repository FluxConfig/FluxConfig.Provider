namespace FluxConfig.Provider.Exceptions;

public class FluxConfigExceptionContext
{
    public Exception? Exception { get; init; }

    public required FluxConfigurationProvider Provider { get; set; }
}