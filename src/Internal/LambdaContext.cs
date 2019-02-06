using System;
using System.Collections;
using Amazon.Lambda.Core;

namespace LambdaNative.Internal
{
    internal class LambdaContext : ILambdaContext
    {
        public IClientContext ClientContext => throw new NotImplementedException();
        public ICognitoIdentity Identity => throw new NotImplementedException();

        public string AwsRequestId { get; set; }
        public string InvokedFunctionArn { get; set; }
        public ILambdaLogger Logger { get; set; }

        public string FunctionName { get; set; }
        public string FunctionVersion { get; set; }
        public string LogGroupName { get; set; }
        public string LogStreamName { get; set; }
        public int MemoryLimitInMB { get; set; }

        public TimeSpan RemainingTime => RemainingTimeFunc();
        public Func<TimeSpan> RemainingTimeFunc { get; set; }

        public LambdaContext(IDictionary initialEnvironmentVariables)
        {
            var memoryLimit = initialEnvironmentVariables["AWS_LAMBDA_FUNCTION_MEMORY_SIZE"] as string;
            int.TryParse(memoryLimit, out var memoryLimitInt);
            MemoryLimitInMB = memoryLimitInt;

            FunctionName = initialEnvironmentVariables["AWS_LAMBDA_FUNCTION_NAME"] as string;
            FunctionVersion = initialEnvironmentVariables["AWS_LAMBDA_FUNCTION_VERSION"] as string;
            LogGroupName = initialEnvironmentVariables["AWS_LAMBDA_LOG_GROUP_NAME"] as string;
            LogStreamName = initialEnvironmentVariables["AWS_LAMBDA_LOG_STREAM_NAME"] as string;
        }
    }
}