using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands;
using Serilog;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using DSharpPlus.Commands.Trees;
using ILogger = Serilog.ILogger;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module
{
    public class Example : IPlugin
    {
        public string Name => "Example Plugin";

        public string Id => "cloudthewolf.example";

        public string Description => "An Example Plugin to demo the system";

        public int Version => 1;

        public System.Version PluginVersion => new(1, 0, 0);

        private ILogger _logger;


        public Task InitializeAsync(PluginContext context, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _logger = context.Logger.ForContext<Example>();
            LoadConfig(context.Configuration);
            RegisterCommands(context.Bot);
            context.Bot.RegisterEventHandlers(Id, events => events.HandleSessionCreated(OnSessionCreated)
                .HandleGuildDownloadCompleted(Downloaded));

            _logger.Information("Example plugin {PluginId} version {PluginVersion} loaded",
                Id, PluginVersion);
            return Task.CompletedTask;
        }

        public Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            _logger?.Information("Example plugin {PluginId} stopped", Id);
            _logger = null;
            return Task.CompletedTask;
        }

        private Task Downloaded(DiscordClient client, GuildDownloadCompletedEventArgs args)
        {
            foreach (var discordGuild in args.Guilds)
            {
                _logger.Information("Guild {GuildName} downloaded", discordGuild.Value.Name);
            }

            return Task.CompletedTask;
        }

        private Task OnSessionCreated(DiscordClient client, SessionCreatedEventArgs args)
        {
            _logger.Information("Session created for plugin");
            return Task.CompletedTask;
        }


        private void RegisterCommands(IBot bot)
        {
            var exampleCommands = CommandBuilder.From(typeof(ExampleCommands));
            bot.RegisterCommand(exampleCommands);
        }

        private void LoadConfig(IConfigurationRoot applicationConfig)
        {
            Options.MySqlHost = applicationConfig.GetValue<string>("SQL:Host");
            Options.MySqlPort = applicationConfig.GetValue<int>("SQL:Port");
            Options.MySqlUsername = applicationConfig.GetValue<string>("SQL:Username");
            Options.MySqlPassword = applicationConfig.GetValue<string>("SQL:Password");
            Options.MySqlDatabase = applicationConfig.GetValue<string>("SQL:Database");
        }
        
    }
}
