using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    public class Program
    {

        public static IConfigurationRoot configuration;
        public static void Main(string[] args)
        {
            Logger.Initialize();
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            Logger.Log.LogInformation("Building service provider");
            var serviceProvider = serviceCollection.BuildServiceProvider();

            CreateHostBuilder(args).Build().Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {

            var configPath = Environment.GetEnvironmentVariable("WORKER_CONFIG_DIR");
            if (!string.IsNullOrEmpty(configPath) && !configPath.EndsWith("/"))
            {
                configPath = $"{configPath}/";
            }
            var config = $"{configPath}appsettings.json";


            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder
                    .AddSerilog(dispose: true);
            }));

            configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile(config, false)
            .Build();

            serviceCollection.AddSerilog();
            serviceCollection.AddSingleton(configuration);

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()  
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();                    
                });
    }
}
