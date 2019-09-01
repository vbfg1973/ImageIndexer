using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Core.Events;
using Infrastructure.Messaging;
using ImageRetrieval.Configuration;
using Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using ImageDetails = Infrastructure.Persistence.ImageDetails;

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

            Console.WriteLine(JsonConvert.SerializeObject(_settings, Formatting.Indented));

            ImageContext imageContext = null;
            ImageRepository repo = null;

            try
            {
                imageContext = new ImageContext(_settings.MongoDbConfig, log);
                repo = new ImageRepository(imageContext);
            }

            catch (Exception e)
            {
                log.Error(e.Message);
            }

            var eventRegistration = new EventBus("ImageRetrieval", "ImageRetrieval", _settings.RabbitSettings.ConnectionString, log);

            var dispatcher = new MessageDispatcher(eventRegistration.AdvancedBus());

            eventRegistration.HandleEvent<ImageDownloaded>(
                (message, info) => Task.Factory.StartNew(() =>
                {
                    var path = message.Body.Path;
                    var fileInfo = new FileInfo(path);
                    log.Information($"{path}: {fileInfo.Length}");
                    var img = repo.GetImage(message.Body.RedditId).Result;
                    var bitmap = (Bitmap)Image.FromFile(path);
                    var hash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());

                    img.Width = bitmap.Width;
                    img.Height = bitmap.Height;
                    img.Filesize = fileInfo.Length;
                    img.Digest = hash;

                    repo.Update(img);
                }),
                TimeSpan.FromSeconds(30).Milliseconds);

            eventRegistration.HandleEvent<ImageFound>(
                (message, info) => Task.Factory.StartNew(() =>
                {
                    log.Information($"{message.Body.RedditId}\t{message.Body.Title}\t{message.Body.Author}\t{message.Body.Subreddit}\t{message.Body.Url}\t{message.Body.CreatedUtc}");

                    // Get file extension
                    var parts = message.Body.Url.Split(".");
                    var extension = parts[parts.Length - 1].ToLower();

                    if (extensionsList.Contains(extension))
                    {
                        log.Information("Downloading");
                        try
                        {
                            var client = new HttpClient();
                            var response = client.GetAsync(message.Body.Url).Result;

                            var dir = Path.Combine("/var/images", message.Body.RedditId.ToLower());

                            if (!Directory.Exists(dir))
                            {
                                log.Information($"Creating directory {dir}");
                                Directory.CreateDirectory(dir);
                            }

                            var path = Path.Combine(dir, string.Format($"{message.Body.RedditId}.{extension}"));
                            response.Content.ReadAsFileAsync(path, true);

                            log.Information($"Saved {message.Body.RedditId} to {path}");

                            var image = new ImageDetails()
                            {
                                Id = message.Body.RedditId,
                                Title = message.Body.Title,
                                Subreddit = message.Body.Subreddit,
                                Author = message.Body.Author,
                                Path = path
                            };

                            repo.Create(image).Wait();

                            var retrieved = new ImageDownloaded()
                            {
                                RedditId = message.Body.RedditId,
                                Path = path,
                                Type = extension
                            };

                            var msg = new MessageEnvelope<ImageDownloaded>(retrieved);
                            dispatcher.Dispatch(msg);
                        }

                        catch (Exception e)
                        {
                            log.Error(e.Message);
                        }
                    }

                    else
                    {
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
