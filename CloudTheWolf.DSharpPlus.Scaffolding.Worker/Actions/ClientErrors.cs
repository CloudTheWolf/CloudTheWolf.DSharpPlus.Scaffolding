using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Actions
{
    internal class ClientErrors
    {

        public static async Task Errored(DiscordClient sender, ClientErrorEventArgs e)
        {
            try
            {
                _ = sender.ReconnectAsync();

            }
            catch (Exception ex)
            {
                Bot.Logger.LogError($"Something Went wrong and we could not reconnect: \n {ex.Message}");
                Environment.Exit(500);
            }
        }
    }
}
