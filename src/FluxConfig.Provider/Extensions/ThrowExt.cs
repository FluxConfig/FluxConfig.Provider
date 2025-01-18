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
            Throw(paramName);
        }
    }

    private static void Throw(string? paramName)
    {
        throw new FluxConfigException("", new ArgumentNullException(paramName));
    }
}