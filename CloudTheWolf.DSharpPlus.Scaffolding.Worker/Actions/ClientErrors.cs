namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker.Actions
{
    internal class ClientErrors
    {

        public static async Task Errored(DiscordClient sender, ClientErrorEventArgs e)
        {
            try
            {
                await sender.ReconnectAsync();

            }
            catch (Exception ex)
            {
                Logger.Log.LogError(ex, "Discord client reconnection failed");
                Environment.Exit(500);
            }
        }
    }
}
