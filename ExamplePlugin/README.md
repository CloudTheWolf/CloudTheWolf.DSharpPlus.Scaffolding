# ExamplePlugin

This project demonstrates the `IPlugin` lifecycle introduced by
`CloudTheWolf.DSharpPlus.Scaffolding.Shared` 4.0.

## Converting an existing plugin

First, update the Shared package reference:

```xml
<PackageReference Include="CloudTheWolf.DSharpPlus.Scaffolding.Shared"
                  Version="4.0.0-beta" />
```

### 1. Replace `InitPlugin` with `InitializeAsync`

Before:

```csharp
public void InitPlugin(
    IBot bot,
    ILogger logger,
    DiscordConfiguration discordConfiguration,
    IConfigurationRoot applicationConfig)
{
    _logger = logger;
    LoadConfig(applicationConfig);
    bot.CommandsList.Add(CommandBuilder.From(typeof(MyCommands)));
    bot.EventHandlerRegistry.Register(events => events
        .HandleSessionCreated(OnSessionCreated));
}
```

After:

```csharp
public Task InitializeAsync(
    PluginContext context,
    CancellationToken cancellationToken = default)
{
    cancellationToken.ThrowIfCancellationRequested();

    _logger = context.Logger.ForContext<MyPlugin>();
    LoadConfig(context.Configuration);

    context.Bot.RegisterCommand(CommandBuilder.From(typeof(MyCommands)));
    context.Bot.RegisterEventHandlers(Id, events => events
        .HandleSessionCreated(OnSessionCreated));

    return Task.CompletedTask;
}
```

The old `InitPlugin(...)` method remains supported for compatibility, but new
plugins should use `InitializeAsync` so startup can perform asynchronous work
and honor worker cancellation.

### 2. Add stable plugin metadata

```csharp
public string Name => "My Plugin";
public string Id => "com.example.my-plugin";
public string Description => "Description of the plugin";
public int Version => 2;
public Version PluginVersion => new(2, 0, 0);
```

`Id` must be stable and unique. It is used for structured logging, duplicate
detection, and named event-handler registration. `PluginVersion` is the full
semantic version; `Version` remains for compatibility.

### 3. Use the context members

| Old argument | New location |
| --- | --- |
| `bot` | `context.Bot` |
| `logger` | `context.Logger` |
| `discordConfiguration` | `context.DiscordConfiguration` |
| `applicationConfig` | `context.Configuration` |

The logger is owned by the Worker. A plugin may enrich it with `ForContext`, but
must not reconfigure or dispose it.

### 4. Add shutdown cleanup

```csharp
public async Task ShutdownAsync(CancellationToken cancellationToken = default)
{
    _pluginLifetime.Cancel();

    if (_backgroundTask is not null)
    {
        await _backgroundTask.WaitAsync(cancellationToken);
    }

    _timer?.Dispose();
    _httpClient?.Dispose();
    _pluginLifetime.Dispose();
}
```

`ShutdownAsync` is called during normal Worker shutdown and before a hot reload.
Dispose timers, subscriptions, streams, database clients, HTTP clients, and any
other plugin-owned resources. Cancel and await background tasks before returning.

Avoid static references to plugin types or instances. A static reference in the
host or another plugin prevents the collectible assembly context from unloading.
The event handlers registered through `RegisterEventHandlers` and commands added
through `RegisterCommand` are released automatically when the client is rebuilt.

## Hot-reload deployment

Enable hot reload in the Worker configuration:

```json
{
  "Plugins": {
    "hotReload": true,
    "reloadDelayMilliseconds": 1500
  }
}
```

Deploy the plugin using this layout:

```text
Plugins/
└── ExamplePlugin/
    ├── ExamplePlugin.dll
    ├── ExamplePlugin.deps.json
    └── dependency.dll
```

Copy dependencies first and `ExamplePlugin.dll` last. The Worker debounces the
file changes, calls `ShutdownAsync`, briefly reconnects Discord, unloads the old
assembly context, and initializes the replacement. Move or delete the directory
to unload the plugin. Renaming it to `ExamplePlugin.disabled` also disables it.

See [`Example.cs`](Example.cs) for the complete implementation.
