using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Dto
{
    public class ImageRetrieve
    {
        public string RedditId { get; set; }
        public string Subreddit { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}
