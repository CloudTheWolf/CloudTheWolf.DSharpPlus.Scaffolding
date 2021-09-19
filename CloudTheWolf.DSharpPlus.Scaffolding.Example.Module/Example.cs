using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Example.Module
{
    public class Example : IPlugin
    {
        public string Name => "Example";

        public string Description => "An Example Plugin to demo the system";

        public int Version => '1';

        public static ILogger<Logger> Logger;

        public void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, IConfigurationRoot applicationConfig)
        {
            Logger = logger;
            Console.WriteLine("Hello World");
            //Console.WriteLine(applicationConfig.Get<string>("a:b"));
        }
    }
}
