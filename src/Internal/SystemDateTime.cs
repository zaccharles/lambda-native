using System;

namespace LambdaNative.Internal
{
    internal class SystemDateTime : IDateTime
    {
        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}
