using System;
using RedditSharp.Things;
using Serilog;

namespace RedditListener.RedditClient.Observers
{
    public class RedditPostObserver : IObserver<RedditSharp.Things.Post>
    {
        private ILogger _logger;
        private string _subredditName;

        public RedditPostObserver(string subredditName, ILogger logger)
        {
            _subredditName = subredditName;
            _logger = logger;

            _logger.Information($"Subscribing to posts on {subredditName}");
        }

        public void OnCompleted()
        {
            _logger.Information($"Finished subscribing to posts on {_subredditName}");
        }

        public void OnError(Exception error)
        {
            _logger.Error($"Error with post subscription on {_subredditName}");
        }

        public void OnNext(Post value)
        {
            _logger.Information(
                $"Post: {value.Id}\t{value.SubredditName}\t{value.AuthorName}\t{value.Title}\t{value.Url}\t{value.CreatedUTC.ToString("o")}");
        }
    }
}