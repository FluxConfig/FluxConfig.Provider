namespace FluxConfig.Provider.Exceptions;

public class FluxConfigException: Exception
{
    public FluxConfigException()
    {
    }

    public FluxConfigException(string? message) : base(message)
    {
    }

    public FluxConfigException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}