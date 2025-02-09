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
                Logger.Log.LogError($"Something Went wrong and we could not reconnect: \n {ex.Message}");
                Environment.Exit(500);
            }
        }
    }
}
