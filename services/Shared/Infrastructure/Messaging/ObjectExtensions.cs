namespace ImageIndexer.Infrastructure.Messaging
{
    public static class ObjectExtensions
    {
        public static string ToStringOrNull(this object @object)
        {
            string str = (string)null;
            if (@object != null)
                str = @object.ToString();
            return str;
        }
    }
}