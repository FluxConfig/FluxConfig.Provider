using Grpc.Core;

namespace FluxConfig.Provider.Exceptions;

/// <summary>
/// Defines a custom exception object that is thrown when RPC parameters used to call FluxConfig.Storage service method are invalid
/// </summary>
public class FluxConfigBadRequestException : FluxConfigException
{
    private readonly IDictionary<string, string> _requestViolations;
    
    /// <summary>
    /// Dictionary of RPC parameters violations where Key is RPC message field and Value is violation description message
    /// </summary>
    public IDictionary<string, string> RpcRequestViolations => _requestViolations;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message">The context specific error message</param>
    /// <param name="fieldViolations">Dictionary of RPC parameters violations where Key is RPC message field and Value is violation description message</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public FluxConfigBadRequestException(
        string? message,
        IDictionary<string, string> fieldViolations,
        RpcException? innerException) : base(message, innerException)
    {
        _requestViolations = fieldViolations;
    }
}