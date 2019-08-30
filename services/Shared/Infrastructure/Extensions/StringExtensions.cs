using System.Text;

namespace Infrastructure.Messaging
{
    public static class StringExtensions
    {
        public static string FullStopBeforeCapital(this string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder(s.Length * 2);
            stringBuilder.Append(s[0]);
            for (int index = 1; index < s.Length; ++index)
            {
                if (char.IsUpper(s[index]) && s[index - 1] != ' ' && s[index - 1] != '.')
                    stringBuilder.Append('.');
                stringBuilder.Append(s[index]);
            }
            return stringBuilder.ToString();
        }
    }
}