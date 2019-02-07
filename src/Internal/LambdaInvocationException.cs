using System;

namespace LambdaNative.Internal
{
    internal class LambdaInvocationException : Exception
    {
        public LambdaInvocationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}