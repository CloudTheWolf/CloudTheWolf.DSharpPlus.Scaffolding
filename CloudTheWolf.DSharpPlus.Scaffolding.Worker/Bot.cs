using System.Threading;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus.Commands;
using Lavalink4NET.Players;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.UserCommands;
using CloudTheWolf.DSharpPlus.Scaffolding.Data;
using Microsoft.Extensions.Configuration;
using ILogger = Serilog.ILogger;
using Logger = CloudTheWolf.DSharpPlus.Scaffolding.Logging.Logger;
using DSharpPlus.Commands.Trees;
using System.Collections.Generic;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Registry;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Net;
using System.Linq;


namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    internal class Bot : IBot
    {
        public DiscordClientBuilder ClientBuilder { get; set; }
        public VoiceNextExtension Voice { get; set; }
        public RestClient Rest { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public CommandsExtension Commands { get; set; }
        public List<CommandBuilder> CommandsList { get; set; } = [];
        public EventHandlerRegistry EventHandlerRegistry { get; private set; } = new();
        public DiscordClient Client { get; set; }
        public LavalinkPlayerOptions LavalinkPlayerOptions { get ; set; }

        private readonly DiscordConfiguration _discordConfiguration = new();
        private readonly List<IPlugin> _initializedPlugins = [];
        private readonly PluginLoaderService _pluginLoaderService = new();
        private readonly SemaphoreSlim _clientLifecycleLock = new(1, 1);
        private PluginHotReloadService _hotReloadService;
        private ILogger _pluginLogger;
        private CancellationToken _stoppingToken;
        private bool _isStopping;

        public async Task RunAsync(CancellationToken stoppingToken, ILogger logger)
        {
            _stoppingToken = stoppingToken;
            _pluginLogger = logger.ForContext<Bot>();
            try
            {
                Logger.Log.LogInformation("Bot starting");
                LoadConfig();
                await _clientLifecycleLock.WaitAsync(stoppingToken).ConfigureAwait(false);
                try
                {
                    await StartClientAsync(stoppingToken).ConfigureAwait(false);
                }
                finally
                {
                    _clientLifecycleLock.Release();
                }

                StartHotReloadWatcher();
                await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                Logger.Log.LogInformation("Bot shutdown requested");
            }
            finally
            {
                _isStopping = true;
                _hotReloadService?.Dispose();
                await _clientLifecycleLock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
                try
                {
                    await StopClientAsync().ConfigureAwait(false);
                }
                finally
                {
                    _clientLifecycleLock.Release();
                    _clientLifecycleLock.Dispose();
                }
            }
        }

        private static void LoadConfig()
        {
            Options.LoadDiscordConfigFromFile = Program.Configuration.GetValue<bool>("UseConfigFile");
            if (Options.LoadDiscordConfigFromFile)
            {
                LoadDiscordConfigFromFile();
            }
            else
            {
                LoadConfigFromDatabase();
            }
        }

        private static void LoadDiscordConfigFromFile()
        {
            Options.Token = Program.Configuration.GetValue<string>("Discord:token");
            Options.Prefix = [Program.Configuration.GetValue<string>("Discord:prefix")];
            Options.EnableDms = Program.Configuration.GetValue<bool>("Discord:enableDms");
            Options.EnableMentionPrefix = Program.Configuration.GetValue<bool>("Discord:enableMentionPrefix");
            Options.DmHelp = Program.Configuration.GetValue<bool>("Discord:dmHelp");
            Options.DefaultHelp = Program.Configuration.GetValue<bool>("Discord:enableDefaultHelp");
            Options.RunInShardMode = Program.Configuration.GetValue<bool>("ShardMode");
            Options.GuildMembers = Program.Configuration.GetValue<bool>("Discord:privilegedIntents:guildMembers");
            Options.GuildPresences = Program.Configuration.GetValue<bool>("Discord:privilegedIntents:guildPresences");
            Options.MessageContents = Program.Configuration.GetValue<bool>("Discord:privilegedIntents:messageContents");
            Options.DebugGuildId = Program.Configuration.GetValue<ulong>("Discord:DebugGuildId");

        }

        private static void LoadConfigFromDatabase()
        {
            var database = DatabaseFactory.CreateDatabase(Program.Configuration);
            var results = database.Query("SELECT * FROM app_config;");
            foreach(var result in results)
            {
                switch(result.name)
                {
                    case "token":
                        Options.Token = result.sValue;
                        break;
                    case "prefix":
                        Options.Prefix = [result.sValue];
                        break;
                    case "enable-dms":
                        Options.EnableDms = Convert.ToBoolean(result.iValue);
                        break;
                    case "enable-mention-prefix":
                        Options.EnableMentionPrefix = Convert.ToBoolean(result.iValue);                        
                        break;
                    case "dm-help":
                        Options.DmHelp = Convert.ToBoolean(result.iValue);
                        break;
                    case "enable-default-help":
                        Options.DefaultHelp = Convert.ToBoolean(result.iValue);
                        break;
                    case "enable-shard-mode":
                        Options.RunInShardMode = Convert.ToBoolean(result.iValue);
                        break;
                }
            }
        }

        private async Task StartClientAsync(CancellationToken cancellationToken, bool loadPlugins = true)
        {
            EventHandlerRegistry = new EventHandlerRegistry();
            CommandsList = [];

            var combinedIntents = DiscordIntents.AllUnprivileged;
            if(Options.GuildMembers)
            {
                combinedIntents |= DiscordIntents.GuildMembers;
            }

            if (Options.GuildPresences)
            {
                combinedIntents |= DiscordIntents.GuildPresences;
            }

            if (Options.MessageContents)
            {
                combinedIntents |= DiscordIntents.MessageContents;
            }
            ClientBuilder = Options.RunInShardMode
                ? DiscordClientBuilder.CreateSharded(Options.Token,
                    combinedIntents)
                : DiscordClientBuilder.CreateDefault(Options.Token,
                    combinedIntents);
            EventHandlerRegistry.Register("worker.session-created",
                events => events.HandleSessionCreated(OnSessionCreated));
            
            ClientBuilder.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            if (loadPlugins)
            {
                InitPlugins();
                await InitPluginsAsync(cancellationToken).ConfigureAwait(false);
            }
            
            var commandsConfiguration = new Action<IServiceProvider, CommandsExtension>((serviceProvider, commandsExtension) =>
            {
                Commands = commandsExtension;
                commandsExtension.AddProcessors([new SlashCommandProcessor(), new MessageCommandProcessor(), new UserCommandProcessor(), new TextCommandProcessor()]);
                foreach (var command in CommandsList)
                {
                    Logger.Log.LogInformation("Adding command {CommandName}", command.Name);
                    commandsExtension.AddCommand(command);
 
                }
            });
            ClientBuilder.UseCommands(commandsConfiguration, new CommandsConfiguration()
            {
                RegisterDefaultCommandProcessors = false
            });
            var events = EventHandlerRegistry.ConfigureAll;
            ClientBuilder.ConfigureEventHandlers(events);
            ClientBuilder.ConfigureLogging(Program.MainLoggingBuilder);

            Client = ClientBuilder.Build();
            await Client.ConnectAsync().ConfigureAwait(false);
            Logger.Log.LogInformation("Bot connected and ready to work with {PluginCount} plugins",
                _initializedPlugins.Count);
        }

        private void StartHotReloadWatcher()
        {
            if (!Program.Configuration.GetValue<bool>("Plugins:hotReload"))
            {
                Logger.Log.LogInformation("Plugin hot reload is disabled");
                return;
            }

            var reloadDelay = Math.Max(
                Program.Configuration.GetValue<int?>("Plugins:reloadDelayMilliseconds") ?? 1500,
                250);
            _hotReloadService = new PluginHotReloadService(
                Constants.PluginsFolder,
                TimeSpan.FromMilliseconds(reloadDelay),
                ReloadPluginsAsync,
                _stoppingToken);
            _hotReloadService.Start();
        }

        private async Task ReloadPluginsAsync(CancellationToken cancellationToken)
        {
            if (_isStopping || cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await _clientLifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (_isStopping || cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Logger.Log.LogInformation("Plugin files changed; reloading plugins");
                await StopClientAsync().ConfigureAwait(false);

                try
                {
                    await StartClientAsync(cancellationToken).ConfigureAwait(false);
                    Logger.Log.LogInformation("Plugin hot reload completed successfully");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Logger.Log.LogError(exception,
                        "Plugin hot reload failed; reconnecting the core bot without plugins");
                    await StopClientAsync().ConfigureAwait(false);
                    await StartClientAsync(cancellationToken, loadPlugins: false).ConfigureAwait(false);
                }
            }
            finally
            {
                _clientLifecycleLock.Release();
            }
        }

        private void InitPlugins()
        {
            Logger.Log.LogInformation("Loading plugins");
            _pluginLoaderService.LoadPlugins();
        }



        private static Task OnSessionCreated(DiscordClient sender, SessionCreatedEventArgs args)
        {
            Logger.Log.LogInformation("Bot session is ready");
            
            return Task.CompletedTask;
        }

        private async Task InitPluginsAsync(CancellationToken cancellationToken)
        {
            foreach (var plugin in _pluginLoaderService.Plugins)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pluginLogger = _pluginLogger
                    .ForContext("PluginId", plugin.Id)
                    .ForContext("PluginName", plugin.Name)
                    .ForContext("PluginVersion", plugin.PluginVersion);
                var context = new PluginContext(
                    this, pluginLogger, _discordConfiguration, Program.Configuration);

                Logger.Log.LogInformation(
                    "Initializing plugin {PluginName} ({PluginId}) version {PluginVersion}",
                    plugin.Name, plugin.Id, plugin.PluginVersion);
                try
                {
                    await plugin.InitializeAsync(context, cancellationToken).ConfigureAwait(false);
                    _initializedPlugins.Add(plugin);
                }
                catch
                {
                    try
                    {
                        await plugin.ShutdownAsync(CancellationToken.None).ConfigureAwait(false);
                    }
                    catch (Exception shutdownException)
                    {
                        Logger.Log.LogError(shutdownException,
                            "Failed to clean up plugin {PluginName} ({PluginId}) after initialization failed",
                            plugin.Name, plugin.Id);
                    }

                    throw;
                }
            }
        }

        private async Task StopClientAsync()
        {
            try
            {
                if (Client is not null)
                {
                    await Client.DisconnectAsync().ConfigureAwait(false);
                    Logger.Log.LogInformation("Bot disconnected");
                }
            }
            catch (Exception exception)
            {
                Logger.Log.LogError(exception, "Failed to disconnect the Discord client cleanly");
            }

            foreach (var plugin in _initializedPlugins.AsEnumerable().Reverse())
            {
                try
                {
                    Logger.Log.LogInformation("Stopping plugin {PluginName} ({PluginId})",
                        plugin.Name, plugin.Id);
                    await plugin.ShutdownAsync(CancellationToken.None).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    Logger.Log.LogError(exception, "Failed to stop plugin {PluginName} ({PluginId})",
                        plugin.Name, plugin.Id);
                }
            }

            _initializedPlugins.Clear();
            _pluginLoaderService.UnloadPlugins();

            // Drop all references to plugin-defined commands and event-handler delegates so
            // collectible assembly load contexts can be reclaimed.
            CommandsList = [];
            EventHandlerRegistry = new EventHandlerRegistry();
            Client = null;
            ClientBuilder = null;
            Commands = null;
        }
    }
}
