using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FakeItEasy;
using LambdaNative.Internal;
using RichardSzalay.MockHttp;
using Xunit;
using LambdaNative.Tests.Extensions;
using Shouldly;

namespace LambdaNative.Tests
{
    public class LambdaRuntimeTests
    {
        private readonly IEnvironment _environment;
        private readonly MockHttpMessageHandler _http;
        private readonly LambdaRuntime _runtime;
        private readonly IDateTime _dateTime;

        public LambdaRuntimeTests()
        {
            _environment = A.Fake<IEnvironment>();
            A.CallTo(() => _environment.GetEnvironmentVariables())
                .Returns(new Dictionary<string, string> { { "AWS_LAMBDA_RUNTIME_API", "test" } });

            _dateTime = A.Fake<IDateTime>();
            _http = new MockHttpMessageHandler();
            _runtime = new LambdaRuntime(_environment, _dateTime, new HttpClient(_http));
        }

        [Fact]
        public void ReportInitializationError_PostsExceptionToCorrectEndpoint()
        {
            // arrange
            var ex = new DivideByZeroException(nameof(ReportInitializationError_PostsExceptionToCorrectEndpoint));

            _http.Expect(HttpMethod.Post, "http://test/2018-06-01/runtime/init/error")
                .WithPartialContent(nameof(DivideByZeroException))
                .WithPartialContent(nameof(ReportInitializationError_PostsExceptionToCorrectEndpoint))
                .Respond(HttpStatusCode.Accepted);

            // act
            _runtime.ReportInitializationError(ex);

            // assert
            _http.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void ReportInvocationError_PostsExceptionToCorrectEndpoint()
        {
            // arrange
            var requestId = Guid.NewGuid().ToString();
            var ex = new DivideByZeroException(nameof(ReportInvocationError_PostsExceptionToCorrectEndpoint));

            _http.Expect(HttpMethod.Post, $"http://test/2018-06-01/runtime/invocation/{requestId}/error")
                .WithPartialContent(nameof(DivideByZeroException))
                .WithPartialContent(nameof(ReportInvocationError_PostsExceptionToCorrectEndpoint))
                .Respond(HttpStatusCode.Accepted);

            // act
            _runtime.ReportInvocationError(requestId, ex);

            // assert
            _http.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void ReportInvocationSuccess_PostsOutputToCorrectEndpoint()
        {
            // arrange
            var requestId = Guid.NewGuid().ToString();
            var outputStream = nameof(ReportInvocationSuccess_PostsOutputToCorrectEndpoint).ToStream();

            _http.Expect(HttpMethod.Post, $"http://test/2018-06-01/runtime/invocation/{requestId}/response")
                .WithPartialContent(nameof(ReportInvocationSuccess_PostsOutputToCorrectEndpoint))
                .Respond(HttpStatusCode.Accepted);

            // act
            _runtime.ReportInvocationSuccess(requestId, outputStream);

            // assert
            _http.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public void GetNextInvocation_GetsInvocation_AndReturnsCorrectInvokeData()
        {
            // arrange
            var requestId = Guid.NewGuid().ToString();
            var traceId = Guid.NewGuid().ToString();
            var functionArn = Guid.NewGuid().ToString();

            var headers = new Dictionary<string, string>
            {
                { "lambda-runtime-aws-request-id", requestId },
                { "lambda-runtime-trace-id", traceId },
                { "lambda-runtime-invoked-function-arn", functionArn },
                { "lambda-runtime-deadline-ms", "1549574242637" }
            };

            _http.Expect(HttpMethod.Get, $"http://test/2018-06-01/runtime/invocation/next")
                .Respond(HttpStatusCode.OK, headers, new StreamContent("input".ToStream()));

            // act
            var invokeData = _runtime.GetNextInvocation();

            // assert
            _http.VerifyNoOutstandingExpectation();
            invokeData.ShouldNotBeNull();
            invokeData.LambdaContext.ShouldNotBeNull();
            invokeData.LambdaContext.AwsRequestId.ShouldBe(requestId);
            invokeData.LambdaContext.InvokedFunctionArn.ShouldBe(functionArn);
            invokeData.LambdaContext.Logger.ShouldNotBeNull();
            invokeData.LambdaContext.RemainingTime.ShouldNotBeNull();
            invokeData.InputStream.ShouldNotBeNull();
            invokeData.InputStream.FromStream().ShouldBe("input");
            invokeData.XAmznTraceId.ShouldBe(traceId);
            invokeData.RequestId.ShouldBe(requestId);
        }

        [Fact]
        public void GetNextInvocation_SetsWorkingRemainingTimeFunc()
        {
            // arrange
            var offsetUtcNow = new DateTimeOffset(2018, 02, 07, 21, 18, 0, TimeSpan.Zero);
            A.CallTo(() => _dateTime.OffsetUtcNow).Returns(offsetUtcNow.AddSeconds(2));

            var deadline = offsetUtcNow.AddSeconds(10).ToUnixTimeMilliseconds().ToString();

            var headers = new Dictionary<string, string>
            {
                { "lambda-runtime-deadline-ms", deadline }
            };

            _http.Expect(HttpMethod.Get, $"http://test/2018-06-01/runtime/invocation/next")
                .Respond(HttpStatusCode.OK, headers, new StreamContent("input".ToStream()));

            // act
            var invokeData = _runtime.GetNextInvocation();

            // assert
            invokeData.LambdaContext.RemainingTime.ShouldBe(TimeSpan.FromSeconds(8));
        }
    }
}