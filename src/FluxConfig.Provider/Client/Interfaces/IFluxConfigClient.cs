namespace FluxConfig.Provider.Client.Interfaces;

internal interface IFluxConfigClient: IDisposable
{
    internal Task<Dictionary<string, string?>> LoadRealTimeConfigAsync(CancellationToken cancellationToken);

    internal Task<Dictionary<string, string?>> LoadVaultConfigAsync(CancellationToken cancellationToken);

    internal Dictionary<string, string?> LoadVaultConfig();
    
    internal string Address { get; }
}