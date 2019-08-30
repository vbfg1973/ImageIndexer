using System;
using System.Text;
using EasyNetQ;

namespace ImageIndexer.Infrastructure.Messaging
{
    public static class MessagePropertiesExtensions
    {
        public static bool GetHeaderAsBool(this MessageProperties messageProperties, string key, bool @default)
        {
            bool result = @default;
            if (messageProperties.Headers.ContainsKey(key) && messageProperties.Headers[key] != null)
            {
                if (messageProperties.Headers[key] is string)
                {
                    result = Convert.ToBoolean(messageProperties.Headers[key] as string);
                }
                else if (messageProperties.Headers[key] is bool)
                {
                    result = (bool)messageProperties.Headers[key];
                }

            }
            return result;
        }

        public static string GetHeaderAsString(this MessageProperties messageProperties, string key)
        {
            string result = null;
            if (messageProperties.Headers.ContainsKey(key) && messageProperties.Headers[key] != null)
            {
                var bytes = messageProperties.Headers[key] as byte[];
                if (bytes != null)
                {
                    result = Encoding.UTF8.GetString(bytes);
                }
                else if (messageProperties.Headers[key] is string)
                {
                    result = (string)messageProperties.Headers[key];
                }
                else
                {
                    throw new Exception("Could not determine header type");
                }
            }
            return result;
        }

        public static int GetHeaderAsInt(this MessageProperties messageProperties, string key)
        {
            return Convert.ToInt32(GetHeaderAsString(messageProperties, key));
        }
    }
}