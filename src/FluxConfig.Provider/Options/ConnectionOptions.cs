namespace FluxConfig.Provider.Options;

/// <summary>
/// Options used to configure connection to FluxConfig.Storage service
/// </summary>
public class ConnectionOptions
{
    internal const string ApiKeyHeader = "X-API-KEY";
    
    /// <summary>
    /// FluxConfig.Storage service url address
    /// </summary>
    public Uri? Address { get; set; }
    
    /// <summary>
    /// Api key used to claim corresponding service configuration data from FluxConfig system
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Creates a new instance of options object with specified parameters
    /// </summary>
    /// <param name="address">FluxConfig.Storage service url address</param>
    /// <param name="apiKey">Api key used to claim corresponding service configuration data from FluxConfig system</param>
    public ConnectionOptions(Uri? address, string? apiKey)
    {
        Address = address;
        ApiKey = apiKey;
    }
    
    /// <summary>
    /// Creates a new instance of options object
    /// </summary>
    public ConnectionOptions() { }
}