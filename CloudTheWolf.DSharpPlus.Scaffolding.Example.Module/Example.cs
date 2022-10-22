using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands;
using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.ApplicationCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module
{
    public class Example : IShardPlugin
    {
        public string Name => "Example Plugin";

        public string Description => "An Example Plugin to demo the system";

        public int Version => '1';

        public static ILogger<Logger> Logger;

        public void InitPlugin(IShardBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, IConfigurationRoot applicationConfig)
        {
            Logger = logger;
            LoadConfig(applicationConfig);
            RegisterCommands(bot);
            Console.WriteLine("Hello World");

        }

        private void RegisterCommands(IShardBot bot)
        {
            bot.SlashCommandsExt.RegisterCommands<ExampleSlashCommands>();
            bot.Commands.RegisterCommands<ExampleCommands>();
            Logger.LogInformation($"{Name}: Registered {nameof(ExampleCommands)}!");
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
