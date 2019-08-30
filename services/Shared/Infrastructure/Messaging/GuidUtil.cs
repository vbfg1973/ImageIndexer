using System;

namespace Infrastructure.Messaging
{
    public static class GuidUtil
    {
        public static Guid Parse(string str)
        {
            bool flag = false;
            Guid result;
            if (!Guid.TryParse(str, out result))
            {
                if (str.Length == 32)
                {
                    str = str.Substring(0, 8) + "-" + str.Substring(8, 4) + "-" + str.Substring(12, 4) + "-" + str.Substring(16, 4) + "-" + str.Substring(20, 12);
                    if (!Guid.TryParse(str, out result))
                        flag = true;
                }
                else
                    flag = true;
            }
            if (flag)
                throw new Exception(string.Format("Could not parse {0} into Guid {1}", (object)0, (object)str));
            return result;
        }

        public static bool Parsable(object obj)
        {
            Guid defaultTo = Guid.NewGuid();
            return GuidUtil.TryParse(obj, defaultTo) != defaultTo;
        }

        public static Guid? GetNullableGuid(string guidAsString)
        {
            Guid? nullable = new Guid?();
            if (GuidUtil.Parsable((object)guidAsString))
                nullable = new Guid?(GuidUtil.Parse(guidAsString));
            return nullable;
        }

        public static Guid? TryParseAsNullable(string s, Guid? @default)
        {
            try
            {
                if (GuidUtil.Parsable((object)s))
                    return new Guid?(GuidUtil.Parse(s));
            }
            catch
            {
            }
            return @default;
        }

        public static Guid ParseOrDefaultTo(string guidString, Guid @default)
        {
            try
            {
                return GuidUtil.Parse(guidString);
            }
            catch
            {
                return @default;
            }
        }

        public static Guid TryParse(object authorizationId, Guid defaultTo)
        {
            if (authorizationId != null)
            {
                if (!string.IsNullOrWhiteSpace(authorizationId.ToString()))
                {
                    try
                    {
                        return GuidUtil.Parse(authorizationId.ToString());
                    }
                    catch
                    {
                        return defaultTo;
                    }
                }
            }
            return defaultTo;
        }
    }
}
