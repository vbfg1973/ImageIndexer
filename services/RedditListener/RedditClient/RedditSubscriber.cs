using System.Collections.Generic;
using System.Threading.Tasks;
using RedditListener.RedditClient;
using RedditSharp;
using Serilog;

namespace RedditListener.RedditRedditClient
{
    public class RedditSubscriber : IRedditSubscriber
    {
        private RedditClientSettings _redditConfig;
        private BotWebAgent _botWebAgent;

        private IDictionary<string, RedditClient.RedditClient> _RedditClients;
        //private IRedditMessenger _redditMessenger;

        private ILogger _logger;

        public RedditSubscriber(BotWebAgent botWebAgent, RedditClientSettings config, ILogger logger)
        {
            _RedditClients = new Dictionary<string, RedditClient.RedditClient>();

            _redditConfig = config;
            _botWebAgent = botWebAgent;
            _logger = logger;
        }

        public async Task Subscribe(string subredditName)
        {
            _logger.Information($"Subscriber: Subscribing to {subredditName}");

            var RedditClient = new RedditClient.RedditClient(_botWebAgent, _logger);
            await RedditClient.Subscribe(subredditName);
            _RedditClients.Add(subredditName, RedditClient);
        }

        public async Task Unsubscribe(string subredditName)
        {
            if (_RedditClients.ContainsKey(subredditName))
            {
                _RedditClients.Remove(subredditName);
            }
        }
    }
}