using System;

namespace Infrastructure.Messaging
{
    [Serializable]
    public abstract class Event : IEvent
    {
        // ReSharper disable InconsistentNaming
        protected Guid _aggregateId;
        // ReSharper restore InconsistentNaming

        protected Event()
        {
            TimestampUtc = DateTime.UtcNow;
        }

        protected Event(Guid id, int version)
        {
            TimestampUtc = DateTime.UtcNow;
            _aggregateId = id;
            Version = version;
        }

        public virtual Guid AggregateId
        {
            get => _aggregateId;
            set => _aggregateId = value;
        }

        public DateTime TimestampUtc { get; set; }

        public int Version { get; set; }

        ///Should be overridden to give a nice logging message if one is requested
        public virtual string ExtraLoggingInfo()
        {
            return null;
        }
    }
}