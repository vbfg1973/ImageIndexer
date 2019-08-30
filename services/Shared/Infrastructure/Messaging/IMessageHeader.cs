using System;

namespace Infrastructure.Messaging
{
    public interface IMessageHeader
    {
        string CorrelationId { get; set; }
        string MessageId { get; set; }
        int RetryCount { get; set; }
        string RetryHandlerTypeName { get; set; }
        string IpAddress { get; set; }
        string ReplyTo { get; set; }
        bool Persistent { get; set; }
        void IncrementRetryCountForHandler(string handlerTypeName);
        string ExceptionMessage { get; set; }
        string StackTrace { get; set; }
        Guid AuthorizationId { get; set; }
        IMessageHeader WithClearedRetries();
        bool SendNotifications { get; set; }
        IMessageHeader WithEmptyStringsNulled();
    }
}