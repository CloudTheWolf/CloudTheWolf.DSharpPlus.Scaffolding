using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;

namespace CloudTheWolf.DSharpPlus.Scaffolding.Worker
{
    public class Program
    {

        public static IConfigurationRoot configuration;

        public static void Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Create service provider
            Log.Information("Building service provider");
            var serviceProvider = serviceCollection.BuildServiceProvider();

            CreateHostBuilder(args).Build().Run();
        }


        private static void ConfigureServices(IServiceCollection serviceCollection)
        {


            var config = "appsettings.json";


            serviceCollection.AddSingleton(LoggerFactory.Create(builder =>
            {
                builder
                    .AddSerilog(dispose: true);
            }));

            configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
            .AddJsonFile(config, false)
            .Build();

            serviceCollection.AddLogging();
            serviceCollection.AddSingleton(configuration);

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()                
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();                    
                });
    }
}
