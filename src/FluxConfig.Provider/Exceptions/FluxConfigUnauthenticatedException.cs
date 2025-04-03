using Grpc.Core;

namespace FluxConfig.Provider.Exceptions;

/// <summary>
/// Defines a custom exception object that is thrown when api key used to authenticate request to the FluxConfig.Storage service
/// is invalid
/// </summary>
public class FluxConfigUnauthenticatedException : FluxConfigException
{
    private readonly string _invalidApiKey;
    
    /// <summary>
    /// Invalid api key used to authenticate request to the FluxConfig.Storage service
    /// </summary>
    public string InvalidApiKey => _invalidApiKey;

    /// <summary>
    /// Creates a new exception object to relay error information to the user.
    /// </summary>
    /// <param name="message">The context specific error message.</param>
    /// <param name="invalidKey">Invalid api key used to authenticate request to the FluxConfig.Storage service</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public FluxConfigUnauthenticatedException(
        string? message,
        string invalidKey,
        RpcException? innerException) : base(message, innerException)
    {
        _invalidApiKey = invalidKey;
    }
}