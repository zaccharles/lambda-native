using System;
using System.Collections;
using System.IO;
using System.Net;
using LitJson;

namespace LambdaNative.Internal
{
    internal class LambdaRuntime : ILambdaRuntime
    {
        private readonly IDictionary _initialEnvironmentVariables;
        private readonly string _endpoint;

        public IEnvironment Environment { get; }

        public bool KeepInvokeLoopRunning()
        {
            return true;
        }

        public LambdaRuntime(IEnvironment environment)
        {
            Environment = environment;
            _initialEnvironmentVariables = environment.GetEnvironmentVariables();

            var api = _initialEnvironmentVariables["AWS_LAMBDA_RUNTIME_API"] as string;
            _endpoint = $"http://{api}/2018-06-01/runtime";
        }

        public InvokeData GetNextInvocation()
        {
            var request = WebRequest.CreateHttp(_endpoint + "/invocation/next");

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to get invocation from runtime API. Status code: {(int)response.StatusCode}.");
                }

                var requestId = response.Headers["lambda-runtime-aws-request-id"];
                var xAmznTraceId = response.Headers["lambda-runtime-trace-id"];
                var invokedFunctionArn = response.Headers["lambda-runtime-invoked-function-arn"];

                var deadlineMs = response.Headers["lambda-runtime-deadline-ms"];
                long.TryParse(deadlineMs, out var deadlineMsLong);
                var deadlineDate = DateTimeOffset.FromUnixTimeMilliseconds(deadlineMsLong);

                var stream = new MemoryStream();
                response.GetResponseStream()?.CopyTo(stream);

                var context = new LambdaContext(_initialEnvironmentVariables)
                {
                    AwsRequestId = requestId,
                    InvokedFunctionArn = invokedFunctionArn,
                    Logger = LambdaLogger.Instance,
                    RemainingTimeFunc = () => deadlineDate.Subtract(DateTimeOffset.UtcNow)
                };

                var invokeData = new InvokeData
                {
                    LambdaContext = context,
                    InputStream = stream,
                    XAmznTraceId = xAmznTraceId,
                    RequestId = requestId
                };

                return invokeData;
            }
        }

        public void ReportInitializationError(Exception exception)
        {
            var json = JsonMapper.ToJson(exception);

            var request = WebRequest.CreateHttp($"{_endpoint}/init/error");
            request.Method = "POST";

            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(json);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine("Failed to report initialization error.");
                    }
                }
            }
        }

        public void ReportInvocationSuccess(string requestId, Stream outputStream)
        {
            var request = WebRequest.CreateHttp($"{_endpoint}/invocation/{requestId}/response");
            request.Method = "POST";

            using (var requestStream = request.GetRequestStream())
            {
                outputStream.CopyTo(requestStream);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Failed to report success for request {requestId}.");
                    }
                }
            }
        }

        public void ReportInvocationError(string requestId, Exception exception)
        {
            var json = JsonMapper.ToJson(exception);

            var request = WebRequest.CreateHttp($"{_endpoint}/invocation/{requestId}/error");
            request.Method = "POST";

            using (var sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(json);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine($"Failed to report error for request {requestId}.");
                    }
                }
            }
        }
    }
}