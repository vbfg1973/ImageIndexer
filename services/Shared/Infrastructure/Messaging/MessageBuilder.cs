using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Infrastructure.Messaging
{
    public static class MessageBuilder
    {
        public static MessageProperties IvendiProperties => new MessageProperties
        {
            AppId = "image-indexer",
            ContentType = "application/json"
        };

        public static IDictionary<string, object> BuildGenericHeaderFromEnvelopeHeader(IMessageHeader header)
        {
            return new Dictionary<string, object>
                   {
                       {Key.RetryCount, header.RetryCount.ToString(CultureInfo.CurrentCulture)},
                       {Key.RetryHandlerTypeName, header.RetryHandlerTypeName ?? string.Empty},
                       {"ReplyTo", header.ReplyTo ?? string.Empty},
                       {"IpAddress", header.IpAddress ?? string.Empty},
                       {"UserId", header.IpAddress ?? string.Empty},
                       {Key.CorrelationId, header.CorrelationId ?? header.MessageId},
                       {"MessageId", header.MessageId ?? string.Empty},
                       {"ExceptionMessage", header.ExceptionMessage ?? string.Empty},
                       {"StackTrace", header.StackTrace ?? string.Empty},
                       {"AuthorizationId", header.AuthorizationId.ToStringOrNull() ?? string.Empty},
                       {"SendNotifications", header.SendNotifications}
                   };
        }

        public static MessageHeader BuildGenericHeaderFromRmqHeader(MessageProperties props)
        {
            int.TryParse(props.GetHeaderAsString("RetryCount"), out int retryCount);
            var domainSessionId = props.GetHeaderAsString("SnowplowDomainSessionId") ?? string.Empty;
            var networkUserId = props.GetHeaderAsString("SnowplowNetworkUserId") ?? string.Empty;

            return new MessageHeader
            {
                CorrelationId = props.CorrelationId,
                MessageId = props.MessageId,
                ReplyTo = props.ReplyTo,
                RetryCount = retryCount,
                RetryHandlerTypeName = props.GetHeaderAsString(Key.RetryHandlerTypeName),
                IpAddress = props.GetHeaderAsString("IpAddress"),
                ExceptionMessage = props.GetHeaderAsString("ExceptionMessage"),
                StackTrace = props.GetHeaderAsString("StackTrace"),
                AuthorizationId = GuidUtil.TryParse(props.GetHeaderAsString("AuthorizationId"), Guid.Empty),
                SendNotifications = props.GetHeaderAsBool("SendNotifications", false)
            };
        }

        public static MessageHeader DeleteRetries(MessageHeader msgHeader)
        {
            msgHeader.RetryCount = 0;
            msgHeader.RetryHandlerTypeName = string.Empty;
            return msgHeader;
        }

        public static IMessage<TMessage> EasyNetQMessageBuilder<TMessage>(IMessageEnvelope<TMessage> msg) where TMessage : class, IMessage
        {
            var message = new Message<TMessage>(msg.Body);

            message.Properties.AppId = "ivendi";
            message.Properties.ContentType = "application/json";
            message.Properties.MessageId = msg.Header.MessageId ?? Guid.NewGuid().ToString();
            message.Properties.CorrelationId = msg.Header.CorrelationId ?? message.Properties.MessageId;
            message.Properties.Headers = BuildGenericHeaderFromEnvelopeHeader(msg.Header);
            message.Properties.ReplyTo = msg.Header.ReplyTo ?? string.Empty;
            if (msg.Header.Persistent)
            {
                message.Properties.DeliveryMode = 2;
            }
            var now = DateTime.UtcNow;
            var t = now - new DateTime(1970, 1, 1).ToUniversalTime();
            var timestamp = (int)t.TotalSeconds;
            message.Properties.Timestamp = timestamp;

            return message;
        }

        public static MessageHeader Map(IMessageHeader msgHeader)
        {
            var header = new MessageHeader
            {
                AuthorizationId = msgHeader.AuthorizationId,
                CorrelationId = msgHeader.CorrelationId,
                ExceptionMessage = msgHeader.ExceptionMessage,
                IpAddress = msgHeader.IpAddress,
                MessageId = msgHeader.MessageId,
                Persistent = msgHeader.Persistent,
                ReplyTo = msgHeader.ReplyTo,
                RetryCount = msgHeader.RetryCount,
                RetryHandlerTypeName = msgHeader.RetryHandlerTypeName,
                StackTrace = msgHeader.StackTrace,
                SendNotifications = msgHeader.SendNotifications
            };
            return header;
        }

        public static class Key
        {
            public const string RetryCount = "RetryCount";
            public const string RetryHandlerTypeName = "RetryHandlerTypeName";
            public const string UndispatchedMessage = "UndispatchedMessage";
            public const string CorrelationId = "CorrelationId";
        }
    }
}
