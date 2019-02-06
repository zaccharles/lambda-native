using System;

namespace LambdaNative.Internal
{
    internal class LambdaUserCodeException : Exception
    {
        public LambdaUserCodeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}