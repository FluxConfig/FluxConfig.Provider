# FluxConfig Provider

**FluxConfig Microsoft.Extensions.Configuration custom Provider.**

Introduces a .NET platform-integrated client for interacting with the FluxConfig system.
Provides automated retrieval and updating of the application's running configuration without
the need to restart the application or interact with the deployment environment.

## Supported runtimes

- .NET 6+
- .NET Core 3+

## Getting Started

### 0. Prerequisites

FluxConfig service complex, including 

- [FluxConfig.Storage](https://github.com/FluxConfig/FluxConfig.Storage)
- [FluxConfig.Management](https://github.com/FluxConfig/FluxConfig.Management)
- [FluxConfig.WebClient](https://github.com/FluxConfig/FluxConfig.WebClient)
  
deployed and ready on your system.

**To learn more about deployment visit [Deployment Guidance](https://github.com/FluxConfig/deployment)**

### 1. Install NuGet [package](https://www.nuget.org/packages/FluxConfig.Provider)

```shell
dotnet add package FluxConfig.Provider
```

or

```shell
Install-Package FluxConfig.Provider
```

### 2. Visit [FluxConfig.WebClient](https://github.com/FluxConfig/FluxConfig.WebClient) to get an api key for your application configuration

TBA.

### 3. Add FluxConfig Provider to your application

**Add FluxConfig Provider to your application IConfiguration via IConfigurationBuilder**

```csharp
IConfigurationRoot config = new ConfigurationBuilder()
            .AddFluxConfig(options =>
            {
                options.ConnectionOptions = new ConnectionOptions
                {
                    Address = new Uri("https://your-fluxconfig-storage-url"),
                    ApiKey = "Your application configuration api key"
                };
            
                options.ConfigurationTag = "Your application configuration tag";           
            }).Build();
```

### 4. Configure FluxConfig Provider if needed

**Configure data polling behaviour**

```csharp
options.PollingOptions = new FluxPollingOptions
        {
            ExceptionBehavior = PollingExceptionBehavior.Ignore,
            RefreshInterval = TimeSpan.FromSeconds(10)
        };
```

**Configure Logging**

```csharp
options.LoggerFactory = LoggerFactory.Create(optionsBuilder =>
                {
                    optionsBuilder.AddConsole();
                    optionsBuilder.SetMinimumLevel(LogLevel.Information);
                });
```

**Configure custom FluxConfig Exception handler if**

```csharp
ExceptionBehavior = PollingExceptionBehavior.Throw
```

**is selected**

```csharp
// Example of application termination handler
configurationBuilder.SetFluxConfigExceptionHandler(ctx =>
    {
        ctx.Logger.LogWarning("Handler: Exception occured while fetching config data: {message}", ctx.Exception?.Message);
        Environment.Exit(-1);
    });
```
