using EasyNetQ;
using EasyNetQ.ConnectionString;
using Serilog;
using JsonSerializer = EasyNetQ.JsonSerializer;

namespace ivendi.Kernel.Receiver
{
    public class BusFactory : IBusFactory
    {
        private static IBusFactory _busFactory;

        private BusFactory()
        {
        }

        public static IBusFactory Instance
        {
            get => _busFactory ?? new BusFactory();
            set => _busFactory = value;
        }

        public IBus Build(string connectionString, string platform, ILogger logger = null, int? prefetchOverride = null)
        {
            var connectionConfiguration = AmqpUriHelper.AmqpUriToMap(connectionString);

            connectionConfiguration.PrefetchCount = (ushort)(prefetchOverride ?? 10);

            if (logger != null) Log.Logger = logger;

            return RabbitHutch.CreateBus(
                connectionConfiguration,
                serviceRegister =>
                {
                    serviceRegister.Register<IConventions>(_ => new Conventions(new LegacyTypeNameSerializer()));
                    serviceRegister.Register<ITypeNameSerializer>(_ => new iVendiEasyNetQSerializer());
                    serviceRegister.Register<ISerializer>(_ => new JsonSerializer());
                });
        }

        public static void Reset()
        {
            Instance = new BusFactory();
        }

        private static class AmqpUriHelper
        {
            public static ConnectionConfiguration AmqpUriToMap(string amqpUri)
            {
                ConnectionConfiguration connectionString = new ConnectionStringParser().Parse(amqpUri);
                return connectionString;
            }
        }
    }
}