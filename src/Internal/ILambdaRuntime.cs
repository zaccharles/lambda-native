using System;
using System.IO;

namespace Lambda.Native.Internal
{
    internal interface ILambdaRuntime
    {
        IEnvironment Environment { get; }
        bool KeepInvokeLoopRunning();
        InvokeData GetNextInvocation();
        void ReportInvocationSuccess(string requestId, Stream outputStream);
        void ReportInvocationError(string requestId, Exception exception);
        void ReportInitializationError(Exception exception);
    }
}