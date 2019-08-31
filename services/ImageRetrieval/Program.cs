using System;
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

                    if (message.Body.Url.EndsWith("jpg") || message.Body.Url.EndsWith("png") || message.Body.Url.EndsWith("jpeg")) {
                        log.Information("Downloading");
                        try {
                            var client = new HttpClient();
                            var response = client.GetAsync(message.Body.Url);

                            var path = Path.Combine("/var/images/", string.Format($"{message.Body.RedditId}.jpg"));
                            response.Content.ReadAsFileAsync(path, false);
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

        public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            string pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
            }
    
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                    (copyTask) =>
                    {
                        fileStream.Close();
                    });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
    
                throw;
            }
        }
    }
}
