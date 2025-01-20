using System.Text;
using FluxConfig.Provider.Exceptions;
using Google.Rpc;
using Grpc.Core;

namespace FluxConfig.Provider.Extensions;

internal static class RpcExceptionExtensions
{
    internal static FluxConfigException GenerateFluxConfigException(this RpcException exc)
    {
        FluxConfigException exception = exc.StatusCode switch
        {
            StatusCode.Internal => new FluxConfigException("FluxConfig.Storage Internal error. Check FluxConfig.Storage logs.", exc),
            StatusCode.NotFound => GenerateNotFoundException(exc),
            StatusCode.InvalidArgument => GenerateBrException(exc),
            StatusCode.Unauthenticated => GenerateUnauthenticatedException(exc),
            StatusCode.Unavailable => new FluxConfigException("Unable to establish connection with FluxConfig.Storage service.", exc),
            _ => new FluxConfigException("", exc)
        };

        return exception;
    }

    private static FluxConfigBadRequestException GenerateBrException(RpcException exc)
    {
        var badRequest = exc.GetRpcStatus()?.GetDetail<BadRequest>();
        var rpcViolations = badRequest?.FieldViolations.ToDictionary(x => x.Field, x => x.Description) ??
                            new Dictionary<string, string>();
        
        StringBuilder violationsBuilder = new StringBuilder();
                    
        foreach (var pair in rpcViolations)
        {
            violationsBuilder.Append($"\n{pair.Key} : {pair.Value}");
        }
        

        return new FluxConfigBadRequestException(
            message: $"Bad request. Invalid rpc arguments: {violationsBuilder}",
            fieldViolations: rpcViolations,
            innerException: exc
        );
    }

    private static FluxConfigNotFoundException GenerateNotFoundException(RpcException exc)
    {
        string invalidTag = exc.GetRpcStatus()?.GetDetail<ErrorInfo>().Metadata["tag"] ?? "";

        return new FluxConfigNotFoundException(
            message: $"Configuration not found. Incorrect configuration tag: <{invalidTag}>",
            invalidTag: invalidTag,
            innerException: exc);
    }

    private static FluxConfigUnauthenticatedException GenerateUnauthenticatedException(RpcException exc)
    {
        string invalidKey = exc.GetRpcStatus()?.GetDetail<ErrorInfo>().Metadata["x-api-key"] ?? "";

        return new FluxConfigUnauthenticatedException(
            message: $"Invalid X-API-KEY: <{invalidKey}> authentication metadata",
            invalidKey: invalidKey,
            innerException: exc
        );
    }
}