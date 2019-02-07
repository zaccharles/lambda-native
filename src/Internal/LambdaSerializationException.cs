using System;

namespace LambdaNative.Internal
{
    internal class LambdaSerializationException : Exception
    {
        public LambdaSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}