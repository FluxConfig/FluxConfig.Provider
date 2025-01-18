namespace FluxConfig.Provider.Client.Interfaces;

internal interface IFluxConfigClient: IDisposable
{
    public Task<Dictionary<string, string?>> LoadRealTimeConfigAsync(CancellationToken cancellationToken);

    public Task<Dictionary<string, string?>> LoadVaultConfigAsync(CancellationToken cancellationToken);

    public Dictionary<string, string?> LoadVaultConfig();
}