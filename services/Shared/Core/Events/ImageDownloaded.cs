using Infrastructure.Messaging;

namespace Core.Events
{
    public class ImageDownloaded : Event
    {
        public string RedditId { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
}