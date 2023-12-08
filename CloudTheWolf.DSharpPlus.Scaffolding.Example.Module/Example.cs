using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.Commands;
using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using CloudTheWolf.DSharpPlus.Scaffolding.Example.Module.ApplicationCommands;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.CommandsNext.Attributes;
using System.Reflection;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module
{
    public class Example : IShardPlugin
    {
        public string Name => "Example Plugin";

        public string Description => "An Example Plugin to demo the system";

        public int Version => '1';

        public static ILogger<Logger> Logger;

        public List<string> MyCommandsList = new List<string>();
        public IShardBot Bot { get; set; }

        public void InitPlugin(IShardBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, IConfigurationRoot applicationConfig)
        {
            Logger = logger;
            LoadConfig(applicationConfig);
            RegisterCommands(bot);
            Console.WriteLine("Hello World");
            Bot = bot;
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

        public void GetCommandNames(Type type)
        {

            // Get all public instance methods
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                // Get the CommandAttribute on the method, if it exists
                var commandAttr = method.GetCustomAttribute<CommandAttribute>();
                if (commandAttr != null)
                {
                    // Add the command name to the list
                    MyCommandsList.Add(commandAttr.Name);
                }
            }

        }

        public void Dispose()
        {
            UnloadPlugin(Bot,Logger,null);
        }

        public void UnloadPlugin(IShardBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration)
        {
            var commands = bot.Commands;
            foreach (var subcommands in commands)
            {
                foreach (var command in subcommands.Value.RegisteredCommands)
                {
                    if(!MyCommandsList.Contains(command.Value.Name)) continue;
                    bot.Commands.UnregisterCommands(command.Value);
                }
            }
        }
    }
}
