using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RedditListener.RedditClient;
using RedditListener.RedditRedditClient;
using RedditSharp;
using Serilog;

namespace RedditListener
{
    class Program
    {
        private static AppSettings _settings;
        private static BotWebAgent _botWebAgent;
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            // Configuration
            ReadConfiguration();
            var logger = ConfigureLogger();

            // BotWebAgent
            _botWebAgent = new BotWebAgent(_settings.RedditListenerSettings.Username,
                _settings.RedditListenerSettings.Password,
                _settings.RedditListenerSettings.ClientId,
                _settings.RedditListenerSettings.ClientSecret,
                _settings.RedditListenerSettings.RedirectUri);

            // Register Services
            RegisterServices();

            // Subscriptions
            foreach (var sr in _settings.RedditListenerSettings.Subreddits)
            {
                logger.Information(sr);
                var subscriber = _serviceProvider.GetService<IRedditSubscriber>();

                try
                {
                    subscriber.Subscribe(sr).Wait();
                }

                catch (Exception e)
                {
                    logger.Error(e.Message);
                }
            }

            Console.ReadLine();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<ILogger>(Log.Logger);
            collection.AddSingleton<RedditClientSettings>(_settings.RedditListenerSettings);
            collection.AddSingleton<IRedditSubscriber, RedditSubscriber>();
            collection.AddSingleton<BotWebAgent>(_botWebAgent);

            _serviceProvider = collection.BuildServiceProvider();
        }

        private static ILogger ConfigureLogger()
        {
            var logger = new LoggerConfiguration()
                .WriteTo
                .Console();

            Log.Logger = logger.CreateLogger();

            return Log.Logger;
        }

        private static void ReadConfiguration()
        {
            _settings = new AppSettings();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            config.Bind("Settings", _settings);
        }
    }
}
