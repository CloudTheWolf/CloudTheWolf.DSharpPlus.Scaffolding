using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    public class Program
    {

        public static IConfigurationRoot Configuration;
        public static Action<ILoggingBuilder> MainLoggingBuilder;
        public static void Main(string[] args)
        {
            Configuration = BuildConfiguration();
            Logger.Initialize(configuration => configuration.ReadFrom.Configuration(Configuration));

            MainLoggingBuilder = builder =>
            {
                builder
                    .ClearProviders()
                    .AddConfiguration(Configuration.GetSection("Logging"))
                    .AddSerilog(Serilog.Log.Logger, dispose: false);
            };

            try
            {
                Logger.Log.LogInformation("Starting the scaffolding worker");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                Logger.Log.LogCritical(exception, "The scaffolding worker terminated unexpectedly");
                throw;
            }
            finally
            {
                Logger.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()  
                .UseSerilog(Serilog.Log.Logger, dispose: false)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(Configuration);
                    services.AddHostedService<Worker>();                    
                });

        private static IConfigurationRoot BuildConfiguration()
        {
            var configPath = Environment.GetEnvironmentVariable("WORKER_CONFIG_DIR");
            var configFile = string.IsNullOrEmpty(configPath)
                ? "appsettings.json"
                : Path.Combine(configPath, "appsettings.json");

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)!.FullName)
                .AddJsonFile(configFile, optional: false)
                .Build();
        }
    }

}
