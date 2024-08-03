using System.Threading;
using Microsoft.Extensions.Hosting;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    public class Worker : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            {
                Logger.Log.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                var bot = new Bot();
                await bot.RunAsync(stoppingToken, Log.Logger);
                await Task.Delay(-1, stoppingToken);
            }
        }
    }
}
