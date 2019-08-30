using System;
using System.Reflection;
using Infrastructure.Messaging;

namespace Core.Events
{
    public class ImageFound : Event
    {
        public string RedditId { get; set; }
        public string Subreddit { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}