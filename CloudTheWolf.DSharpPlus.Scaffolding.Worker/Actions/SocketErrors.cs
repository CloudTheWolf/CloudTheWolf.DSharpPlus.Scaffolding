namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Actions
{

    

    internal class SocketErrors
    {
        public static async Task Closed(DiscordClient sender, SocketClosedEventArgs e)
        {
            try
            {
                await sender.ReconnectAsync();
            }
            catch (Exception ex)
            {
                Logger.Log.LogError(ex, "Discord socket reconnection failed after the socket closed");
                Environment.Exit(500);
            }
            
        }

        public static async Task Errored(DiscordClient sender, SocketErrorEventArgs e)
        {
            try
            {
                await sender.ReconnectAsync();
            }
            catch (Exception ex)
            {
                Logger.Log.LogError(ex, "Discord socket reconnection failed after a socket error");
                Environment.Exit(500);
            }
        }
    }
}
