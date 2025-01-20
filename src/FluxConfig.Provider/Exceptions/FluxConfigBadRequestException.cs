using Grpc.Core;

namespace FluxConfig.Provider.Exceptions;

public class FluxConfigBadRequestException: FluxConfigException
{
    private IDictionary<string, string> _requestViolations;
    
    public FluxConfigBadRequestException(string? message, IDictionary<string, string> fieldViolations, RpcException? innerException) : base(message, innerException)
    {
        _requestViolations = fieldViolations;
    }
}