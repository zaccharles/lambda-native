using System;
using System.IO;
using Amazon.Lambda.Core;
using FakeItEasy;
using LambdaNative.Internal;
using Shouldly;
using Xunit;

namespace LambdaNative.Tests
{
    public class LambdaBootstrapTests
    {
        private readonly ILambdaRuntime _runtime;
        private readonly IHandlerRunner _runner;
        private readonly LambdaBootstrap _bootstrap;

        public LambdaBootstrapTests()
        {
            _runtime = A.Fake<ILambdaRuntime>();
            _runner = A.Fake<IHandlerRunner>();
            _bootstrap = new LambdaBootstrap(_runtime, _runner);
        }

        [Fact]
        public void Initialize_InitializesHandlerRunner()
        {
            // arrange

            // act
            _bootstrap.Initialize();

            // assert
            A.CallTo(() => _runner.Initialize()).MustHaveHappened();
        }

        [Fact]
        public void Initialize_ReportsHandlerInitializationErrors_AndRethrowsException()
        {
            // arrange
            A.CallTo(() => _runner.Initialize()).Throws<DivideByZeroException>();

            // act
            var ex = Record.Exception(() => _bootstrap.Initialize());

            // assert
            ex.ShouldNotBeNull();
            ex.ShouldBeOfType<DivideByZeroException>();

            A.CallTo(() => _runtime.ReportInitializationError(A<DivideByZeroException>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void Run_SetsXrayEnvironmentVariableBeforeCallingHandle()
        {
            // arrange
            A.CallTo(() => _runtime.KeepInvokeLoopRunning()).ReturnsNextFromSequence(true, false);

            var invokeData = new InvokeData
            {
                XAmznTraceId = Guid.NewGuid().ToString(),
                InputStream = new MemoryStream(),
                LambdaContext = A.Fake<ILambdaContext>()
            };

            A.CallTo(() => _runtime.GetNextInvocation()).Returns(invokeData);

            var environment = A.Fake<IEnvironment>();
            A.CallTo(() => _runtime.Environment).Returns(environment);

            // act
            _bootstrap.Run();

            // assert
            A.CallTo(() => environment.SetEnvironmentVariable("_X_AMZN_TRACE_ID", invokeData.XAmznTraceId))
                .MustHaveHappened()
                .Then(A.CallTo(() => _runner.Handle(invokeData.InputStream, invokeData.LambdaContext)).MustHaveHappened());
        }

        [Fact]
        public void Run_ReportsInvocationSuccess()
        {
            // arrange
            A.CallTo(() => _runtime.KeepInvokeLoopRunning()).ReturnsNextFromSequence(true, false);

            var invokeData = new InvokeData { RequestId = Guid.NewGuid().ToString() };
            A.CallTo(() => _runtime.GetNextInvocation()).Returns(invokeData);

            var environment = A.Fake<IEnvironment>();
            A.CallTo(() => _runtime.Environment).Returns(environment);

            var outputStream = new MemoryStream();
            A.CallTo(() => _runner.Handle(A<Stream>.Ignored, A<ILambdaContext>.Ignored))
                .Returns(outputStream);

            // act
            _bootstrap.Run();

            // assert
            A.CallTo(() => _runtime.ReportInvocationSuccess(invokeData.RequestId, outputStream))
                .MustHaveHappened();
        }

        [Fact]
        public void Run_ReportsInvocationError()
        {
            // arrange
            A.CallTo(() => _runtime.KeepInvokeLoopRunning()).ReturnsNextFromSequence(true, false);

            var invokeData = new InvokeData { RequestId = Guid.NewGuid().ToString() };
            A.CallTo(() => _runtime.GetNextInvocation()).Returns(invokeData);

            var environment = A.Fake<IEnvironment>();
            A.CallTo(() => _runtime.Environment).Returns(environment);

            A.CallTo(() => _runner.Handle(A<Stream>.Ignored, A<ILambdaContext>.Ignored))
                .Throws<DivideByZeroException>();

            // act
            _bootstrap.Run();

            // assert
            A.CallTo(() => _runtime.ReportInvocationError(invokeData.RequestId, A<DivideByZeroException>.Ignored))
                .MustHaveHappened();
        }
    }
}