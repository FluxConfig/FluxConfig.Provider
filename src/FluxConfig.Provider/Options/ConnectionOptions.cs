namespace FluxConfig.Provider.Options;

public class ConnectionOptions
{
    public Uri? Address { get; set; }
    public string? ApiKey { get; set; }

    public ConnectionOptions(Uri? address, string? apiKey)
    {
        Address = address;
        ApiKey = apiKey;
    }

    public ConnectionOptions(string address, string? apiKey)
    {
        Address = new Uri(address);
        ApiKey = apiKey;
    }

    public ConnectionOptions()
    {
        
    }
}