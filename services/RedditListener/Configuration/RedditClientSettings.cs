using System.Collections.Generic;

namespace RedditListener
{
    public class RedditClientSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public List<string> Subreddits { get; set; }
    }
}