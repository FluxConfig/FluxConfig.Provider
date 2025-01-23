using Microsoft.Extensions.Logging;

namespace FluxConfig.Provider.Logging;

internal static partial class LoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "[{CurTime}] Start executing."
        )]
    internal static partial void StartProviderExecution(this ILogger logger, DateTime curTime);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "[{CurTime}] Exception occured while fetching {ConfigType} configuration data: {ExceptionMessage}"
        )]
    internal static partial void LogException(this ILogger logger, string? configType, DateTime curTime, string? exceptionMessage);
    
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "[{CurTime}] <Exception Handler> : Exception occured while fetching configuration data: {ExceptionMessage}"
    )]
    internal static partial void LogDefaultHandlerException(this ILogger logger, DateTime curTime, string? exceptionMessage);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "[{CurTime}] Started fetching {ConfigType} configuration data from address {StorageAddress}"
        )]
    internal static partial void LogConfigDataFetchStart(this ILogger logger, DateTime curTime, string? configType,
        string? storageAddress);
    
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "[{CurTime}] Finished fetching {ConfigType} configuration data from address {StorageAddress}"
    )]
    internal static partial void LogConfigDataFetchFinish(this ILogger logger, DateTime curTime, string? configType,
        string? storageAddress);
}