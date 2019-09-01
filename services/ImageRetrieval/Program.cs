using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
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
            List<string> extensionsList = new List<string>()
            {
                "jpg",
                "jpeg",
                "png"
            };

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
                    log.Information($"{message.Body.RedditId}\t{message.Body.Title}\t{message.Body.Author}\t{message.Body.Subreddit}\t{message.Body.Url}\t{message.Body.CreatedUtc}");

                    // Get file extension
                    var parts = message.Body.Url.Split(".");
                    var extension = parts[parts.Length - 1].ToLower();

                    if (extensionsList.Contains(extension)) {
                        log.Information("Downloading");
                        try {
                            var client = new HttpClient();
                            var response = client.GetAsync(message.Body.Url).Result;

                            var dir = Path.Combine("/var/images", message.Body.Subreddit.ToLower());

                            if (!Directory.Exists(dir))
                            {
                                log.Information($"Ceating directory {dir}");
                                Directory.CreateDirectory(dir);
                            }

                            var path = Path.Combine(dir, string.Format($"{message.Body.RedditId}.{extension}"));
                            response.Content.ReadAsFileAsync(path, false);

                            log.Information($"Saved {message.Body.RedditId} to {path}");
                        }

                        catch (Exception e) {
                            log.Error(e.Message);
                        }
                    }

                    else {
                        log.Information("Ignoring");
                    }
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
