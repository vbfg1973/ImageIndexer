using System;
using System.Net.Http;
using System.Text;
using Core.Dto;
using Newtonsoft.Json;
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
            var image = new ImageRetrieve()
            {
                RedditId = value.Id,
                Url = value.Url.AbsoluteUri,
                Subreddit = value.SubredditName,
                Title = value.Title,
                Author = value.AuthorName,
                CreatedUtc = value.CreatedUTC
            };

            try
            {
                var client = new HttpClient();
                var response = client.PostAsync("http://imageapi:80/api/imageretrieve",
                    new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json")).Result;
            }

            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}