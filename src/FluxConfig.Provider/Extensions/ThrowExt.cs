using FluxConfig.Provider.Exceptions;

namespace FluxConfig.Provider.Extensions;

internal static class ThrowExt
{
    internal static void ThrowIfNull(
        object? argument,
        string? paramName = null)
    {
        if (argument is null)
        {
            ThrowNull(paramName);
        }
    }

    private static void ThrowNull(string? paramName)
    {
        throw new FluxConfigException("", new ArgumentNullException(paramName));
    }
}