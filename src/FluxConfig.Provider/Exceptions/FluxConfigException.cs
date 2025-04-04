namespace FluxConfig.Provider.Exceptions;

/// <summary>
/// Defines a custom exception object that is thrown when an error occurs during <see cref="FluxConfigurationProvider"/> execution 
/// </summary>
public class FluxConfigException : Exception
{
    /// <summary>
    /// Creates a new exception object to relay error information to the user.
    /// </summary>
    /// <param name="message">The context specific error message</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public FluxConfigException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}