using System;
using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands;
using Serilog;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Trees;
using ILogger = Serilog.ILogger;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module
{
    public class Example : IPlugin
    {
        public string Name => "Example Plugin";

        public string Description => "An Example Plugin to demo the system";

        public int Version => '1';


        public IBot Bot { get; set; }

        internal CommandsExtension Commands { get; set; }


        public void InitPlugin(IBot bot, ILogger logger, DiscordConfiguration discordConfiguration, IConfigurationRoot applicationConfig)
        {
            Logger.Initialize();
            LoadConfig(applicationConfig);
            RegisterCommands(bot);
            Logger.Log.LogInformation("Example Plugin Loaded");
            bot.EventHandlerRegistry.Register(e => e.HandleSessionCreated(OnSessionCreated));
            Bot = bot;
        }

        private async Task OnSessionCreated(DiscordClient client, SessionCreatedEventArgs args)
        {
            //if(args.Message.Author.IsBot) return;
            Logger.Log.LogInformation("Session Created For Plugin");
        }


        private void RegisterCommands(IBot bot)
        {
            var exampleCommands = CommandBuilder.From(typeof(ExampleCommands));
            bot.CommandsList.Add(exampleCommands);
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
