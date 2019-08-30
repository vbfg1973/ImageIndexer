using System;
using System.Collections.Generic;
using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Infrastructure.Messaging
{
    public class MessageDispatcher
    {
        public MessageDispatcher(IAdvancedBus bus)
        {
            Bus = bus;
        }

        public IAdvancedBus Bus { get; }

        public void Dispatch<TMessage>(IMessageEnvelope<TMessage> message) where TMessage : class, IMessage
        {
            DispatchWithRoutingKey(message, message.GetRoutingKey());
        }

        private void DispatchWithRoutingKey<TMessage>(IMessageEnvelope<TMessage> message, string routingKey) where TMessage : class, IMessage
        {
            try
            {
                var easyNetQMessage = MessageBuilder.EasyNetQMessageBuilder(message);
                string exchangeName = message.ExchangeName();
                var exchange = Bus.ExchangeDeclare(exchangeName, ExchangeType.Direct);
                Bus.Publish(exchange, routingKey, false, easyNetQMessage);
            }
            catch (Exception ex)
            {
                var msg = $"Could not send message of type '{typeof(TMessage).Name}' to exchange '{message.ExchangeName()}'. Error was: {ex.Message}";
                throw new EasyNetQException(msg, ex);
            }
        }

        public void Reply<TMessage>(IMessageEnvelope<TMessage> message) where TMessage : class, IMessage
        {
            DispatchWithRoutingKey(message, message.GetReplyRoutingKey());
        }

        public void DispatchMultiple<TMessage>(IList<IMessageEnvelope<TMessage>> msgs) where TMessage : class, IMessage
        {
            foreach (var msg in msgs)
            {
                Dispatch(msg);
            }
        }

        public void Dispose()
        {
            Bus?.Dispose();
        }
    }
}