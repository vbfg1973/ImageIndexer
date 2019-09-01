using System;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using Serilog;

namespace Infrastructure.Messaging
{
    public class EventBus
    {
        private readonly string _queuePostFix;
        private static IAdvancedBus _evtBus;

        public EventBus(string queuePostFix, string platformName, string connectionString, ILogger log)
        {
            _queuePostFix = queuePostFix;
            _evtBus = BusFactory.Instance.Build(connectionString, platformName, log).Advanced;
        }

        public void HandleEvent<T>(Func<IMessage<T>, MessageReceivedInfo, Task> onMessage, int? ttlMs, bool passive = false, bool durable = true) where T : class, IEvent
        {
            var queue = CreateConsumerQueue<T>(_queuePostFix, ttlMs, passive, durable);
            _evtBus.Consume<T>(queue, onMessage);
        }

        private IQueue CreateConsumerQueue<T>(string queuePostFix, int? messageTtlMs, bool passive, bool durable) where T : IEvent
        {
            var exchangeName = FullStopBeforeCapital(typeof(T).Name);

            var queue = _evtBus.QueueDeclare($"{exchangeName}.{queuePostFix}", passive: passive, durable: durable, perQueueMessageTtl: messageTtlMs);
            var exchange = _evtBus.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            var binding = _evtBus.Bind(exchange, queue, exchangeName);

            return queue;
        }

        public IAdvancedBus AdvancedBus()
        {
            return _evtBus;
        }

        private static string FullStopBeforeCapital(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder(s.Length * 2);
            stringBuilder.Append(s[0]);

            for (int index = 1; index < s.Length; ++index)
            {
                if (char.IsUpper(s[index]) && s[index - 1] != ' ' && s[index - 1] != '.')
                    stringBuilder.Append('.');
                stringBuilder.Append(s[index]);
            }
            return stringBuilder.ToString();
        }
    }
}