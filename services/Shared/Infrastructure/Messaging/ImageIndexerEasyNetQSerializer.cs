using System;
using EasyNetQ;

namespace Infrastructure.Messaging
{
    public class ImageIndexerEasyNetQSerializer : ITypeNameSerializer
    {
        public Type DeSerialize(string typeName)
        {
            Console.WriteLine("The Typename: " + typeName);
            var nameParts = typeName.Split(':');
            if (nameParts.Length != 2)
            {
                throw new EasyNetQException("type name {0}, is not a valid EasyNetQ type name. Expected Type:Assembly", typeName);
            }

            string typeString = nameParts[0] + ", " + nameParts[1];
            typeString = typeString.Replace("_", ".");
            var type = Type.GetType(typeString);

            if (type == null)
            {
                throw new EasyNetQException(
                    "Cannot find type {0}",
                    typeName);
            }
            return type;
        }

        public string Serialize(Type type)
        {
            if (type == null)
            {
                throw new ArgumentException("Type cannot be null");
            }

            var typeString = type.FullName?.Replace('.', '_') + ":" + type.Assembly.GetName().Name.Replace('.', '_');
            return typeString;
        }
    }
}