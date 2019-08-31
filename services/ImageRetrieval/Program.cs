using System;
using System.IO;
using System.Threading.Tasks;
using Core.Events;
using Infrastructure.Messaging;
using ImageRetrieval.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace ImageRetrieval
{
    class Program
    {
        private static IConfiguration _configuration;
        private static AppSettings _settings;

        static void Main(string[] args)
        {
            _settings = LoadConfiguration();
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();

            Task.Delay(20000).Wait();

            var eventRegistration = new EventBus("ImageRetrieval", "ImageRetrieval", _settings.RabbitSettings.ConnectionString, log);

            eventRegistration.HandleEvent<ImageFound>(
                (message, info) => Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"{message.Body.RedditId}\t{message.Body.Author}\t{message.Body.Subreddit}\t{message.Body.Url}");
                }),
                TimeSpan.FromSeconds(30).Milliseconds);

            Console.ReadLine();
        }

        private static AppSettings LoadConfiguration()
        {
            //var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
            AppSettings appSettings = new AppSettings();
            _configuration.Bind("Settings", appSettings);

            return appSettings;
        }

    }
}
