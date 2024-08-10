using System.Threading;
using Serilog.Core;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using CloudTheWolf.DSharpPlus.Scaffolding.Worker.Services;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using DSharpPlus.Commands;
using Lavalink4NET.Players;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.UserCommands;
using CloudTheWolf.DSharpPlus.Scaffolding.Data;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Serilog.ILogger;


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

        public static ILogger Logger;

        public async Task RunAsync(CancellationToken stoppingToken, ILogger logger)
        {
            
            Logger = logger;
            Logger.Information("Bot Starting!");
            
            
            LoadConfig();
            InitClient();
            InitPlugins();
            Client = ClientBuilder.Build();
            CreateDiscordClient();
            InitCommands();
            await Client.ConnectAsync();
            Logger.Information("Ready to work!");
            await Task.Delay(-1, stoppingToken);
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

        private void InitClient()
        {
            ClientBuilder = Options.RunInShardMode ? DiscordClientBuilder.CreateSharded(Options.Token,DiscordIntents.AllUnprivileged) :
                DiscordClientBuilder.CreateDefault(Options.Token, DiscordIntents.AllUnprivileged);
            
            ClientBuilder.ConfigureLogging(Program.MainLoggingBuilder);
        }

        private void InitPlugins()
        {
            Logger.Information("Load Plugins");
            PluginLoaderService.LoadPlugins();
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
            Commands.AddProcessors([
                new SlashCommandProcessor(), new MessageCommandProcessor(), new UserCommandProcessor()
            ]);

        }

        private static Task OnSeasonCreated(DiscordClient sender, SessionCreatedEventArgs args)
        {
            Logger.Information($"Bot Ready!");
            
            return Task.CompletedTask;
        }

        private async void InitCommands()
        {
            foreach (var plugin in PluginLoaderService.Plugins)
            {
                Logger.Information($"Initialise Plugin {plugin.Name}");
                plugin.InitPlugin(this, Logger, _config, Program.Configuration);
            }

            //await Commands.RefreshAsync();

        }
    }
}
