using System.Threading.Tasks;

namespace RedditListener.RedditClient
{
    public interface IRedditSubscriber
    {
        Task Subscribe(string subreddit);
    }
}