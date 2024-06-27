using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.SystemCommands;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Configuration;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    internal class Bot : IBot
    {
        public DiscordClient Client { get; set; }
        public VoiceNextExtension Voice { get; set; }
        public DiscordRestClient Rest { get; set; }
        public CommandsNextExtension Commands { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public LavalinkConfiguration LavalinkConfig { get; set; }
        public SlashCommandsExtension SlashCommandsExt { get; set; }


        private static DiscordConfiguration _config;
        private static dynamic _myConfig;
        internal static readonly PluginLoader PluginLoader = new PluginLoader();

        public static ILogger<Logger> Logger;

        public async Task RunAsync(CancellationToken stoppingToken, ILogger<Logger> logger)
        {
            Logger = logger;
            Logger.LogInformation("Bot Starting!");
            LoadConfig();
            SetDiscordConfig();
            CreateDiscordClient();
            CreateClientCommandConfiguration();
            RegisterCommands();
            InitPlugins();
            await Client.ConnectAsync();
            await Task.Delay(-1, stoppingToken);
        }

        private void InitPlugins()
        {
            PluginLoader.LoadPlugins();

            foreach (var plugin in PluginLoader.Plugins)
            {
                Console.WriteLine($"Load {plugin.Value.Name}");
                plugin.Value.InitPlugin(this, Logger, _config, Program.configuration);
            }
        }

        private static void LoadConfig()
        {
            Options.Token = Program.configuration.GetValue<string>("Discord:token");
            Options.Prefix = new string[] { Program.configuration.GetValue<string>("Discord:prefix") };
            Options.EnableDms = Program.configuration.GetValue<bool>("Discord:enableDms");
            Options.EnableMentionPrefix = Program.configuration.GetValue<bool>("Discord:enableMentionPrefix");
            Options.DmHelp = Program.configuration.GetValue<bool>("Discord:dmHelp");
            Options.DefaultHelp = Program.configuration.GetValue<bool>("Discord:enableDefaultHelp");

        }


        private void CreateClientCommandConfiguration()
        {
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = Options.Prefix,
                EnableDms = Options.EnableDms,
                EnableMentionPrefix = Options.EnableMentionPrefix,
                DmHelp = Options.DmHelp,
                EnableDefaultHelp = Options.DefaultHelp
            };

            Commands = Client.UseCommandsNext(commandsConfig);
        }

        private void CreateDiscordClient()
        {
            Client = new DiscordClient(_config);
            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1)
            });
            Interactivity = Client.GetInteractivity();
            Client.Ready += OnClientReady;
            SlashCommandsExt = Client.UseSlashCommands();
            
        }

        private static void SetDiscordConfig()
        {
            _config = new DiscordConfiguration
            {
                Token = Options.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LoggerFactory = Logging.Logger.LoggerFactory,

            };
        }

        public void RegisterCommands()
        {
            Commands.RegisterCommands<PluginManagerCommands>();
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs readyEventArgs)
        {
            Logger.LogInformation($"Bot Ready!");
            
            return Task.CompletedTask;
        }
    }
}
