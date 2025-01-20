using Grpc.Core;

namespace FluxConfig.Provider.Exceptions;

public class FluxConfigNotFoundException: FluxConfigException
{
    private readonly string _invalidConfigurationTag; 
    
    public string InvalidConfigurationTag
    {
        get => _invalidConfigurationTag;
    }
    
    public FluxConfigNotFoundException(string? message, string invalidTag, RpcException? innerException) : base(message, innerException)
    {
        _invalidConfigurationTag = invalidTag;
    }
}