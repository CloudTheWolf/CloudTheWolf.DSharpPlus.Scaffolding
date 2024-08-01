using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using DSharpPlus.Commands;
using Lavalink4NET.Players;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.UserCommands;
using CloudTheWolf.DSharpPlus.Scaffolding.Data;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    internal class Bot : IBot
    {
        public DiscordClientBuilder ClientBuilder { get; set; }
        public VoiceNextExtension Voice { get; set; }
        public DiscordRestClient Rest { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public CommandsExtension Commands { get; set; }
        public DiscordClient Client { get; set; }
        public LavalinkPlayerOptions LavalinkPlayerOptions { get ; set; }

        private static DiscordConfiguration _config;
        private static dynamic _myConfig;
        internal static PluginLoaderService PluginLoaderService = new();

        public static ILogger<Logger> Logger;

        public async Task RunAsync(CancellationToken stoppingToken, ILogger<Logger> logger)
        {
            Logger = logger;
            Logger.LogInformation("Bot Starting!");            
            LoadConfig();
            InitClient();
            CreateDiscordClient();           
            InitPlugins();
            InitCommands();
            await Client.ConnectAsync();
            await Task.Delay(-1, stoppingToken);
        }

        private static void LoadConfig()
        {
            Options.LoadDiscordConfigFromFile = Program.configuration.GetValue<bool>("UseConfigFile");
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
            Options.Token = Program.configuration.GetValue<string>("Discord:token");
            Options.Prefix = [Program.configuration.GetValue<string>("Discord:prefix")];
            Options.EnableDms = Program.configuration.GetValue<bool>("Discord:enableDms");
            Options.EnableMentionPrefix = Program.configuration.GetValue<bool>("Discord:enableMentionPrefix");
            Options.DmHelp = Program.configuration.GetValue<bool>("Discord:dmHelp");
            Options.DefaultHelp = Program.configuration.GetValue<bool>("Discord:enableDefaultHelp");
            Options.RunInShardMode = Program.configuration.GetValue<bool>("ShardMode");

        }

        private static void LoadConfigFromDatabase()
        {
            var database = DatabaseFactory.CreateDatabase(Program.configuration);
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
                        Options.EnableMentionPrefix = Convert.ToBoolean(result.iValue);                        break;
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

        private void InitClient()
        {
            ClientBuilder = Options.RunInShardMode ? DiscordClientBuilder.CreateSharded(Options.Token,DiscordIntents.AllUnprivileged) :
                DiscordClientBuilder.CreateDefault(Options.Token, DiscordIntents.AllUnprivileged);
            Client = ClientBuilder.Build();
        }

        private void InitPlugins()
        {
            Logger.LogInformation("Load Plugins");
            PluginLoaderService.LoadPlugins();
            foreach (var plugin in PluginLoaderService.Plugins)
            {
                Logger.LogInformation($"Initilise Plugin {plugin.Name}");
                plugin.InitPlugin(this, Logger, _config, Program.configuration);
            }
        }



        private async void CreateDiscordClient()
        {            
            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(1)
            });
            Interactivity = Client.GetInteractivity();
            Client.SessionCreated += OnSeasonCreated;
            CommandsConfiguration commandsConfiguration = new()
            {
                RegisterDefaultCommandProcessors = false,                
            };

            Commands = Client.UseCommands(commandsConfiguration);
            await Commands.AddProcessorsAsync([
                new SlashCommandProcessor(), new MessageCommandProcessor(), new UserCommandProcessor()
            ]);

        }

        private static Task OnSeasonCreated(DiscordClient sender, SessionCreatedEventArgs args)
        {
            Logger.LogInformation($"Bot Ready!");
            
            return Task.CompletedTask;
        }

        private void InitCommands()
        {
            Commands.RefreshAsync();
        }
    }
}
