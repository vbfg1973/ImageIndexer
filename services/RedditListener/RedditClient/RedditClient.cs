﻿using System.Threading;
using System.Threading.Tasks;
using RedditListener.RedditClient.Observers;
using RedditSharp;
using RedditSharp.Things;
using Serilog;

namespace RedditListener.RedditClient
{
    public class RedditClient
    {
        private ILogger _logger;
        private BotWebAgent _botWebAgent;
        private Reddit _reddit;
        private Subreddit _subreddit;

        private RedditPostObserver _posts;
        private RedditCommentObserver _comments;

        public RedditClient(BotWebAgent botWebAgent, ILogger logger)
        {
            _botWebAgent = botWebAgent;
            _logger = logger;
        }

        public async Task Subscribe(string subredditName)
        {
            _reddit = new Reddit(_botWebAgent, false);
            _subreddit = await _reddit.GetSubredditAsync(subredditName);

            await _subreddit.SubscribeAsync();
            SubscribePosts(subredditName);
//            SubscribeComments(subredditName);
        }

        private void SubscribePosts(string subredditName)
        {
            _logger.Information($"Attempting to subscribe to posts on {subredditName}");
            _posts = new RedditPostObserver(subredditName, _logger);

            var postsStream = _subreddit.GetPosts().Stream();
            postsStream.Subscribe(_posts);

            CancellationToken postCancellationToken = new CancellationToken();
            postsStream.Enumerate(postCancellationToken);
        }

        private void SubscribeComments(string subredditName)
        {
            _logger.Information($"Attempting to subscribe to comments on {subredditName}");
            _comments = new RedditCommentObserver(subredditName, _logger);

            var postsStream = _subreddit.GetComments().Stream();
            postsStream.Subscribe(_comments);

            CancellationToken postCancellationToken = new CancellationToken();
            postsStream.Enumerate(postCancellationToken);
        }
    }
}