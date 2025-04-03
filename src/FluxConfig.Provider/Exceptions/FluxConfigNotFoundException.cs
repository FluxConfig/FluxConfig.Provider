using Grpc.Core;

namespace FluxConfig.Provider.Exceptions;

/// <summary>
/// Defines a custom exception object that is thrown when configuration tag corresponding to FluxConfigs service configuration is invalid.
/// </summary>
public class FluxConfigNotFoundException : FluxConfigException
{
    private readonly string _invalidConfigurationTag;
    
    /// <summary>
    /// Invalid configuration tag used to claim configuration from FluxConfig.Storage service
    /// </summary>
    public string InvalidConfigurationTag => _invalidConfigurationTag;

    /// <summary>
    /// Creates a new exception object to relay error information to the user.
    /// </summary>
    /// <param name="message">The context specific error message</param>
    /// <param name="invalidTag">nvalid configuration tag used to claim configuration from FluxConfig.Storage service</param>
    /// <param name="innerException"> The exception that caused the current exception.</param>
    public FluxConfigNotFoundException(
        string? message,
        string invalidTag,
        RpcException? innerException) : base(message, innerException)
    {
        _invalidConfigurationTag = invalidTag;
    }
}