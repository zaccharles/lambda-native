using System;

namespace LambdaNative.Internal
{
    internal class LambdaInitializationException : Exception
    {
        public LambdaInitializationException(string message) : base(message)
        {
        }

        public LambdaInitializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}