using System;

namespace Lambda.Native.Internal
{
    internal class LambdaValidationException : Exception
    {
        public LambdaValidationException(string message) : base(message)
        {
        }

        public LambdaValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}