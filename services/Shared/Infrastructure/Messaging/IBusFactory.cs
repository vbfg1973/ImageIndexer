using EasyNetQ;
using Serilog;

namespace ivendi.Kernel.Receiver
{
    public interface IBusFactory
    {
        IBus Build(string connectionString, string platform, ILogger logger = null, int? prefetchOverride = null);

    }
}