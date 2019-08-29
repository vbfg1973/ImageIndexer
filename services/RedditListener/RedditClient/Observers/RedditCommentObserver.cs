using System;
using RedditSharp.Things;
using Serilog;

namespace RedditListener.RedditClient.Observers
{
    public class RedditCommentObserver : IObserver<RedditSharp.Things.Comment>
    {
        private ILogger _logger;
        private string _subredditName;

        public RedditCommentObserver(string subredditName, ILogger logger)
        {
            _subredditName = subredditName;
            _logger = logger;

            _logger.Information($"Subscribing to comments on {subredditName}");
        }

        public void OnCompleted()
        {
            _logger.Information($"Finished subscribing to posts on {_subredditName}");
        }

        public void OnError(Exception error)
        {
            _logger.Error($"Error with post subscription on {_subredditName}");
        }

        public void OnNext(Comment value)
        {
            _logger.Information(
                $"Comment: {value.Id}\t{value.Subreddit}\t{value.Body.Length}\t{value.CreatedUTC.ToString("o")}");
        }
    }
}