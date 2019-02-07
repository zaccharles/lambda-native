using System.IO;
using System.Text;

namespace LambdaNative.Tests.Extensions
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        public static string FromStream(this Stream s)
        {
            var ms = new MemoryStream();
            var position = s.Position;
            s.CopyTo(ms);
            s.Position = position;

            return Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)s.Length);
        }
    }
}
