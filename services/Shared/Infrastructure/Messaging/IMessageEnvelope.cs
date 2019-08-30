namespace Infrastructure.Messaging
{
    public interface IMessageEnvelope<T> where T : IMessage
    {
        IMessageHeader Header { get; set; }

        T Body { get; set; }

        string GetReplyRoutingKey();

        string GetRoutingKey();

        string ExchangeName();

        IMessageEnvelope<T> WithEmptyHeaderStringsNulled();
    }
}