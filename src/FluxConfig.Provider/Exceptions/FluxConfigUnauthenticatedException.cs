using Grpc.Core;

namespace FluxConfig.Provider.Exceptions;

public class FluxConfigUnauthenticatedException: FluxConfigException
{
    private readonly string _invalidApiKey;
    
    public FluxConfigUnauthenticatedException(string? message, string invalidKey, RpcException? innerException) : base(message, innerException)
    {
        _invalidApiKey = invalidKey;
    }

    public string InvalidApiKey
    {
        get => _invalidApiKey;
    }
}