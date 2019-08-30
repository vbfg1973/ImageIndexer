using System;
using System.Linq;

namespace ImageIndexer.Infrastructure.Messaging
{
    public class MessageHeader : IMessageHeader
    {
        public string CorrelationId { get; set; }
        public string MessageId { get; set; }
        public int RetryCount { get; set; }
        public string RetryHandlerTypeName { get; set; }
        public string IpAddress { get; set; }
        public string ReplyTo { get; set; }
        public bool Persistent { get; set; }

        public MessageHeader()
        {

        }

        public MessageHeader(string correlationId)
        {
            CorrelationId = correlationId;
        }
        public void IncrementRetryCountForHandler(string handlerTypeName)
        {
            if (!string.IsNullOrWhiteSpace(RetryHandlerTypeName) && RetryHandlerTypeName?.Split('.').Last() != handlerTypeName?.Split('.').Last())
            {
                throw new InvalidOperationException(
                    $"This message is already associated with retries using a different handler ({RetryHandlerTypeName})");
            }
            RetryHandlerTypeName = !string.IsNullOrWhiteSpace(RetryHandlerTypeName) ? RetryHandlerTypeName : handlerTypeName;
            RetryCount++;
        }

        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public Guid AuthorizationId { get; set; }

        public IMessageHeader WithClearedRetries()
        {
            var msgHeader = new MessageHeader
            {
                AuthorizationId = AuthorizationId,
                CorrelationId = CorrelationId,
                ExceptionMessage = ExceptionMessage,
                IpAddress = IpAddress,
                MessageId = MessageId,
                Persistent = Persistent,
                ReplyTo = ReplyTo,
                RetryCount = 0,
                RetryHandlerTypeName = null,
                StackTrace = StackTrace,
                SendNotifications = SendNotifications
            };
            return msgHeader;
        }

        public bool SendNotifications { get; set; }

        public IMessageHeader WithEmptyStringsNulled()
        {
            return new MessageHeader
            {
                AuthorizationId = AuthorizationId,
                CorrelationId = CorrelationId,
                ExceptionMessage = string.IsNullOrWhiteSpace(ExceptionMessage) ? null : ExceptionMessage,
                IpAddress = string.IsNullOrWhiteSpace(IpAddress) ? null : IpAddress,
                MessageId = string.IsNullOrWhiteSpace(MessageId) ? null : MessageId,
                Persistent = Persistent,
                ReplyTo = string.IsNullOrWhiteSpace(ReplyTo) ? null : ReplyTo,
                RetryHandlerTypeName = string.IsNullOrWhiteSpace(RetryHandlerTypeName) ? null : RetryHandlerTypeName,
                RetryCount = RetryCount,
                SendNotifications = SendNotifications,
                StackTrace = string.IsNullOrWhiteSpace(StackTrace) ? null : StackTrace
            };
        }
    }
}