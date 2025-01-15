namespace FluxConfig.Provider.Options;

public class ConnectionOptions
{
    internal const string ApiKeyHeader = "X-API-KEY";
    public Uri? Address { get; set; }
    public string? ApiKey { get; set; }

    public ConnectionOptions(Uri? address, string? apiKey)
    {
        Address = address;
        ApiKey = apiKey;
    }
    public ConnectionOptions()
    {
        
    }
}