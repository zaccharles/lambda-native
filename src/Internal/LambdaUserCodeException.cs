using System;

namespace Lambda.Native.Internal
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