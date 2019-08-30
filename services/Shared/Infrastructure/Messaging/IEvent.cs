using System;

namespace Infrastructure.Messaging
{
    public interface IEvent : IMessage
    {
        Guid AggregateId { get; set; }
        DateTime TimestampUtc { get; }
        int Version { get; set; }
        string ExtraLoggingInfo();
    }
}