using System.Threading;
using Microsoft.Extensions.Hosting;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    public class Worker : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.Log.LogInformation("Worker running at {Timestamp}", DateTimeOffset.Now);

            var bot = new Bot();
            await bot.RunAsync(stoppingToken, Log.Logger).ConfigureAwait(false);
        }
    }
}
