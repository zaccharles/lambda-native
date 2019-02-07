using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using LitJson;

namespace LambdaNative.Internal
{
    internal class LambdaRuntime : ILambdaRuntime
    {
        private readonly IDictionary _initialEnvironmentVariables;
        private readonly HttpClient _http;
        private readonly JsonWriter _jsonWriter;

        public IEnvironment Environment { get; }

        public bool KeepInvokeLoopRunning()
        {
            return true;
        }

        public LambdaRuntime(IEnvironment environment, HttpClient http)
        {
            Environment = environment;
            _initialEnvironmentVariables = environment.GetEnvironmentVariables();

            var endpoint = _initialEnvironmentVariables["AWS_LAMBDA_RUNTIME_API"] as string;
            http.BaseAddress = new Uri($"http://{endpoint}/2018-06-01/runtime/");

            _http = http;
            _jsonWriter = new JsonWriter { LowerCaseProperties = true, PrettyPrint = true };
        }

        public InvokeData GetNextInvocation()
        {
            using (var response = _http.GetAsync("invocation/next").GetAwaiter().GetResult())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to get invocation from runtime API. Status code: {(int)response.StatusCode}.");
                }

                var requestId = response.Headers.GetValues("lambda-runtime-aws-request-id").FirstOrDefault();
                var xAmznTraceId = response.Headers.GetValues("lambda-runtime-trace-id").FirstOrDefault();
                var invokedFunctionArn = response.Headers.GetValues("lambda-runtime-invoked-function-arn").FirstOrDefault();

                var deadlineMs = response.Headers.GetValues("lambda-runtime-deadline-ms").FirstOrDefault();
                long.TryParse(deadlineMs, out var deadlineMsLong);
                var deadlineDate = DateTimeOffset.FromUnixTimeMilliseconds(deadlineMsLong);

                var responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                var inputStream = new MemoryStream();
                responseStream.CopyTo(inputStream);

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
                    InputStream = inputStream,
                    XAmznTraceId = xAmznTraceId,
                    RequestId = requestId
                };

                return invokeData;
            }
        }

        public void ReportInitializationError(Exception exception)
        {
            var json = ToJson(new ErrorResponse(exception));

            using (var response = _http.PostAsync("init/error",
                new StringContent(json)).GetAwaiter().GetResult())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine("Failed to report initialization error.");
                }
            }
        }

        public void ReportInvocationSuccess(string requestId, Stream outputStream)
        {
            using (var response = _http.PostAsync($"invocation/{requestId}/response",
                new StreamContent(outputStream)).GetAwaiter().GetResult())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Failed to report success for request {requestId}.");
                }
            }
        }

        public void ReportInvocationError(string requestId, Exception exception)
        {
            var json = ToJson(new ErrorResponse(exception));

            using (var response = _http.PostAsync($"invocation/{requestId}/error",
                new StringContent(json)).GetAwaiter().GetResult())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine($"Failed to report error for request {requestId}.");
                }
            }
        }

        private string ToJson(object obj)
        {
            _jsonWriter.Reset();
            JsonMapper.ToJson(obj, _jsonWriter);
            return _jsonWriter.ToString();
        }
    }

    public class ErrorResponse
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string[] StackTrace { get; set; }
        public ErrorResponse InnerException { get; set; }
        public ErrorResponse Cause { get; set; }
        public ErrorResponse[] Causes { get; set; }

        public ErrorResponse(Exception ex)
        {
            if (ex == null) return;

            ErrorType = ex.GetType().Name;
            ErrorMessage = ex.Message;
            StackTrace = ex.StackTrace?.Split('\n').ToArray();
            InnerException = ex.InnerException != null ? new ErrorResponse(ex) : null;

            if (!(ex is AggregateException aggregateException)) return;

            Causes = aggregateException.InnerExceptions?.Select(x => new ErrorResponse(x)).ToArray();
            Cause = Causes?.FirstOrDefault();
        }
    }
}