using System;
using Infrastructure.Messaging.Serialization;
using Newtonsoft.Json;

namespace Infrastructure.Messaging
{
    public class MessageEnvelope<T> : IMessageEnvelope<T> where T : IMessage
    {
        public MessageEnvelope()
        {
        }

        public MessageEnvelope(T body, IMessageHeader header = null)
        {
            Header = header ?? new MessageHeader();
            Body = body;
        }

        public MessageEnvelope(T body, MessageHeader header)
        {
            Header = header ?? new MessageHeader();
            Body = body;
        }


        [JsonConverter(typeof(ConcreteTypeConverter<MessageHeader>))]
        public IMessageHeader Header { get; set; }

        public T Body { get; set; }

        public string GetRoutingKey()
        {
            if (Header.RetryCount == 0)
            {
                return Body.GetType().Name.FullStopBeforeCapital();
            }
            return Body.GetType().Name.FullStopBeforeCapital() + "." + Header.RetryHandlerTypeName;
        }

        public string ExchangeName()
        {
            return Body.GetType().Name.FullStopBeforeCapital();
        }

        public string GetReplyRoutingKey()
        {
            if (!String.IsNullOrWhiteSpace(Header.ReplyTo))
            {
                return Header.ReplyTo;
            }
            return GetRoutingKey();
        }

        public IMessageEnvelope<T> WithEmptyHeaderStringsNulled()
        {
            return new MessageEnvelope<T>(Body, Header.WithEmptyStringsNulled());
        }
    }

    public class MessageEnvelope
    {
        public static MessageEnvelope<T> Wrap<T>(T body) where T : IMessage
        {
            return new MessageEnvelope<T>(body);
        }
    }
}