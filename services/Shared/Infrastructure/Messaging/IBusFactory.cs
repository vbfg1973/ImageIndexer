using EasyNetQ;
using Serilog;

namespace Infrastructure.Messaging
{
    public interface IBusFactory
    {
        IBus Build(string connectionString, string platform, ILogger logger = null, int? prefetchOverride = null);

    }
}