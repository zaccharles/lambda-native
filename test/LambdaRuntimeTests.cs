using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using FakeItEasy;
using LambdaNative.Internal;
using RichardSzalay.MockHttp;
using Xunit;

namespace LambdaNative.Tests
{
    public class LambdaRuntimeTests
    {
        private readonly IEnvironment _environment;
        private readonly MockHttpMessageHandler _http;
        private readonly LambdaRuntime _runtime;

        public LambdaRuntimeTests()
        {
            _environment = A.Fake<IEnvironment>();
            A.CallTo(() => _environment.GetEnvironmentVariables())
                .Returns(new Dictionary<string, string> { { "AWS_LAMBDA_RUNTIME_API", "test" } });

            _http = new MockHttpMessageHandler();
            _runtime = new LambdaRuntime(_environment, new HttpClient(_http));
        }

        [Fact]
        public void ReportInitializationError_PostsExceptionToCorrectEndpoint()
        {
            // arrange
            var ex = new DivideByZeroException(nameof(ReportInitializationError_PostsExceptionToCorrectEndpoint));

            _http.Expect("http://test/2018-06-01/runtime/init/error")
                .WithPartialContent(nameof(DivideByZeroException))
                .WithPartialContent(nameof(ReportInitializationError_PostsExceptionToCorrectEndpoint))
                .Respond(HttpStatusCode.OK);

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

            _http.Expect($"http://test/2018-06-01/runtime/invocation/{requestId}/error")
                .WithPartialContent(nameof(DivideByZeroException))
                .WithPartialContent(nameof(ReportInvocationError_PostsExceptionToCorrectEndpoint))
                .Respond(HttpStatusCode.OK);

            // act
            _runtime.ReportInvocationError(requestId, ex);

            // assert
            _http.VerifyNoOutstandingExpectation();
        }
    }
}