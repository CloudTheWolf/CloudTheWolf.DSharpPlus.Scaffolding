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
                sender.ReconnectAsync(true);

            }
            catch (Exception ex)
            {

                Environment.Exit(500);
            }
        }
    }
}
