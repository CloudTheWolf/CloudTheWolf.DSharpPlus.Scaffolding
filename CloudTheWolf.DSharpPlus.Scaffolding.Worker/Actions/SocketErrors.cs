using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Actions
{
    using Microsoft.Extensions.Logging;
    using Serilog;

    internal class SocketErrors
    {
        public static async Task Closed(DiscordClient sender, SocketCloseEventArgs e)
        {
            try
            {
                await sender.ReconnectAsync(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Environment.Exit(500);
            }
            
        }

        public static async Task Errored(DiscordClient sender, SocketErrorEventArgs e)
        {
            try
            {
                await sender.ReconnectAsync(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Environment.Exit(500);
            }
        }
    }
}
