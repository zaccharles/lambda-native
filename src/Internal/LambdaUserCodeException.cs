using System;

namespace LambdaNative.Internal
{
    internal class LambdaUserCodeException : Exception
    {
        public bool IsInvokeException { get; }

        public LambdaUserCodeException(string message, Exception innerException, bool isInvokeException)
            : base(message, innerException)
        {
            IsInvokeException = isInvokeException;
        }
    }
}