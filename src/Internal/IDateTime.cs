using System;

namespace LambdaNative.Internal
{
    internal interface IDateTime
    {
        DateTimeOffset OffsetUtcNow { get; }
        DateTimeOffset OffsetNow { get; }
        DateTime UtcNow { get; }
        DateTime Now { get; }
    }
}
