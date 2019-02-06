using System;

namespace LambdaNative.Internal
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