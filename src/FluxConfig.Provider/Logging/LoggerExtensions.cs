using Microsoft.Extensions.Logging;

namespace FluxConfig.Provider.Logging;

internal static partial class LoggerExtensions
{
    
    # region Error
    
    [LoggerMessage(
        Level = LogLevel.Error,
        EventId = 4000,
        Message = "Exception occured while fetching {ConfigType} configuration data: {ExceptionMessage}"
        )]
    internal static partial void LogException(this ILogger logger,
        string? configType,
        string? exceptionMessage);
    
    
    [LoggerMessage(
        Level = LogLevel.Error,
        EventId = 4001,
        Message = "<Exception Handler> : Exception occured while fetching configuration data: {ExceptionMessage}"
    )]
    internal static partial void LogDefaultHandlerException(this ILogger logger, string? exceptionMessage);
    
    #endregion
    
    # region Info
    
    [LoggerMessage(
        Level = LogLevel.Information,
        EventId = 2000,
        Message = "Start executing for address {StorageAddress}."
        )]
    internal static partial void StartProviderExecution(this ILogger logger, string? storageAddress);
    
    #endregion
    
    # region Debug

    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 1000,
        Message = "Started fetching {ConfigType} configuration data."
        )]
    internal static partial void LogConfigDataFetchStart(this ILogger logger, string? configType);
    
    
    [LoggerMessage(
        Level = LogLevel.Debug,
        EventId = 1001,
        Message = "Finished fetching {ConfigType} configuration data."
    )]
    internal static partial void LogConfigDataFetchFinish(this ILogger logger, string? configType);
    
    #endregion
    
}