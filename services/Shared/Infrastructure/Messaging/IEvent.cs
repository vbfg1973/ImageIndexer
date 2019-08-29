using System;

namespace ivendi.Kernel.Receiver
{
    public interface IEvent : IMessage
    {
        Guid AggregateId { get; set; }
        DateTime TimestampUtc { get; }
        int Version { get; set; }
        string ExtraLoggingInfo();
    }
}