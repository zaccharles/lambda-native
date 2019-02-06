using System.IO;
using Amazon.Lambda.Core;

namespace Lambda.Native.Internal
{
    internal class InvokeData
    {
        internal string RequestId { get; set; }
        internal string XAmznTraceId { get; set; }
        internal AwsCredentials AwsCredentials { get; set; }
        internal Stream InputStream { get; set; }
        internal ILambdaContext LambdaContext { get; set; }
    }
}