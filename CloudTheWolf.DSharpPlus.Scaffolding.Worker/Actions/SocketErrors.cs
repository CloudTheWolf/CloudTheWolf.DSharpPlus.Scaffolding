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
                Log.Error(ex.Message);
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
                Log.Error(ex.Message);
                Environment.Exit(500);
            }
        }
    }
}
