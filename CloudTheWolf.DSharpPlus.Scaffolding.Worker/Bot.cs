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


namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    internal class Bot : IBot
    {
        public DiscordClientBuilder ClientBuilder { get; set; }
        public VoiceNextExtension Voice { get; set; }
        public DiscordRestClient Rest { get; set; }
        public InteractivityExtension Interactivity { get; set; }
        public CommandsExtension Commands { get; set; }
        public List<CommandBuilder> CommandsList { get; set; }
        public EventHandlerRegistry EventHandlerRegistry { get; } = new();
        public DiscordClient Client { get; set; }
        public LavalinkPlayerOptions LavalinkPlayerOptions { get ; set; }

        private static DiscordConfiguration _config;
        private static dynamic _myConfig;
        internal static PluginLoaderService PluginLoaderService = new();

        private static ILogger LoggerItem;

        public async Task RunAsync(CancellationToken stoppingToken, ILogger logger)
        {
            LoggerItem = logger;
            Logger.Log.LogInformation("Bot Starting!");
            LoadConfig();
            InitClient();
            Client = ClientBuilder.Build();
            await Client.ConnectAsync();
            Logger.Log.LogInformation("Ready to work!");
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
            Options.Intents = Program.Configuration.GetValue<int>("Discord:IntentIds");
            Options.DebugGuildId = Program.Configuration.GetValue<ulong>("Discord.DebugGuildId");

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
            EventHandlerRegistry.Register(e => e.HandleSessionCreated(OnSeasonCreated));
            

            CommandsList = [];
            InitPlugins();
            InitCommands();
            
            var commandsConfiguration = new Action<IServiceProvider, CommandsExtension>((serviceProvider, commandsExtension) =>
            {
                commandsExtension.AddProcessors([new SlashCommandProcessor(), new MessageCommandProcessor(), new UserCommandProcessor(), new TextCommandProcessor()]);
                foreach (var command in CommandsList)
                {
                    commandsExtension.AddCommand(command);
                }
            });
            ClientBuilder.UseCommands(commandsConfiguration, new CommandsConfiguration()
            {
                RegisterDefaultCommandProcessors = false,
                DebugGuildId = Options.DebugGuildId
            });
            var events = EventHandlerRegistry.ConfigureAll;
            ClientBuilder.ConfigureEventHandlers(events);
            ClientBuilder.ConfigureLogging(Program.MainLoggingBuilder);
        }

        private void InitPlugins()
        {
            Logger.Log.LogInformation("Load Plugins");
            PluginLoaderService.LoadPlugins();
        }



        private static Task OnSeasonCreated(DiscordClient sender, SessionCreatedEventArgs args)
        {
            Logger.Log.LogInformation($"Bot Ready!");
            
            return Task.CompletedTask;
        }

        private void InitCommands()
        {
            foreach (var plugin in PluginLoaderService.Plugins)
            {
                Logger.Log.LogInformation($"Initialise Plugin {plugin.Name}");
                plugin.InitPlugin(this, LoggerItem, _config, Program.Configuration);
            }
        }
    }
}
