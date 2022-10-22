﻿using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.SlashCommands;
using Emzi0767.Utilities;
using Microsoft.Extensions.Configuration;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    internal class ShardBot : IShardBot
    {
        public DiscordShardedClient Client { get; set; }
        public IReadOnlyDictionary<int, VoiceNextExtension> Voice { get; set; }
        public DiscordRestClient Rest { get; set; }
        public IReadOnlyDictionary<int,CommandsNextExtension> Commands { get; set; }
        public IReadOnlyDictionary<int, InteractivityExtension> Interactivity { get; set; }
        public IReadOnlyDictionary<int, LavalinkConfiguration> LavalinkConfig { get; set; }
        public IReadOnlyDictionary<int, SlashCommandsExtension> SlashCommandsExt { get; set; }


        private static DiscordConfiguration _config;
        private static dynamic _myConfig;
        private static readonly PluginLoader PluginLoader = new PluginLoader();

        public static ILogger<Logger> Logger;

        public async Task RunAsync(CancellationToken stoppingToken, ILogger<Logger> logger)
        {
            Logger = logger;
            Logger.LogInformation("Bot Starting!");
            LoadConfig();
            SetDiscordConfig();
            CreateDiscordClient();
            CreateClientCommandConfiguration();
            InitPlugins();
            await Client.StartAsync();
            await Task.Delay(-1, stoppingToken);
        }

        private void InitPlugins()
        {
            PluginLoader.LoadShardPlugins();

            foreach (var plugin in PluginLoader.ShardPlugins)
            {
                plugin.InitPlugin(this, Logger, _config, Program.configuration);
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
            

            Commands = Client.UseCommandsNextAsync(commandsConfig).Result;
        }

        private void CreateDiscordClient()
        {
            Client = new DiscordShardedClient(_config);
            Interactivity = Client.GetInteractivityAsync().Result;
            Client.Ready += OnClientReady;
            SlashCommandsExt = Client.UseSlashCommandsAsync().Result;
            
        }

        private static void SetDiscordConfig()
        {
            _config = new DiscordConfiguration
            {
                Token = Options.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All,
                LoggerFactory = Logging.Logger.LoggerFactory,

            };
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs readyEventArgs)
        {
            Logger.LogInformation($"Bot Ready!");
            
            return Task.CompletedTask;
        }
    }
}