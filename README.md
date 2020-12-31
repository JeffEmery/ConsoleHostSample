## Sample Console Host

This project demonstrates a generic host .NET Core from a Console App Visual Studio 2019 template.

The majority of the configuration, boilerplate and default setup comes from [Microsoft - .NET Generic Host in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0)

##### Packages

Configuration is provided with [Microsoft.Extensions.Configuration](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/)

The generic host is provided with [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting/)

#### Sample Project

This sample provides a demonstration of the default services provided by [`CreateDefaultBuilder`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0#default-builder-settings). The default builder sets up these services by default so there is very little boilerplate code. 

##### [Default Builder](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-5.0#default-builder-settings)

The initialization boils down to the following method. The configuration also demonstrates using the configuration `IOptions` pattern and adding a derived hosted service `BackgroundService : IHostedService1`.

```csharp
Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
{
    services.Configure<WorkerOptions>(context.Configuration.GetSection(WorkerOptions.Settings));
    services.AddHostedService<LoopingHostedService>();
});
```

##### [Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0)

Configuration is automatically loaded from `appsettings.json` and `appsettings.{Environment}.json`

##### [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-5.0)

Dependency injection is provided automatically, with service registration being done in `ConfigureServices(IServiceCollection services)`.

```csharp
ConfigureServices((context, services) =>
{
    services.Configure<WorkerOptions>(context.Configuration.GetSection(WorkerOptions.Settings));
    services.AddHostedService<LoopingHostedService>();
});
```

##### [Binding `IOptions`](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0)

And example of binding settings from `appsettings.json` to a typed object that uses `IOptions<T>` to inject settings into classes is demonstrated.

##### [Environment Variables](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-5.0)

Environment variables are provided by default with prefix `DOTNET_`. Variables are set explicitly in `launchSettings.json` or via project debug properties. Once set,  `IHostEnvironment` is available through dependency injection.

##### [Logging](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-5.0)

Logging is automatically setup, providing the `ILogger<>` interface with dependency injection. Logger configuration is automatically read from `appsettings.json`

##### [`BackgroundService` and `IHostedService`](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice)

There are two implementations of a background service. Either way works; however, `BackgroundService` is probably better until or unless there is some reason driving the necessity of IHostedService

There is quite a bit to [know and understand about `Task` and `async/await`](https://www.pluralsight.com/guides/using-task-run-async-await). Some of it boils down to IO bound work should be awaited while CPU bound work should be queued with `Task.Run`.

##### [Cancellation Tokens](https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads)

Cancellation tokens are used with `BackgroundService` to signal the shutdown of the host. They could also be used to signal a stop to the service without shutting down the host.