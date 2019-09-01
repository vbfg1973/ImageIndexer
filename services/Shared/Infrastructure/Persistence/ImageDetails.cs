using Shipwreck.Phash;

namespace Infrastructure.Persistence
{
    public class ImageDetails
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subreddit { get; set; }
        public string Author { get; set; }
        public string Path { get; set; }
        public int Score { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long Filesize { get; set; }
        public Digest Digest { get; set; }
    }
}