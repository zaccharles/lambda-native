using System;
using Xunit;
using Amazon.Lambda.Core;
using FakeItEasy;
using System.Reflection;
using LambdaNative.Internal;

namespace LambdaNative.Tests
{
    public class HandlerRunnerTests
    {
        public class TestHandler : IHandler, IHandler<string, string>
        {
            public virtual ILambdaSerializer Serializer { get; }

            public virtual string Handle(string input, ILambdaContext context)
            {
                return null;
            }
        }

        [Fact]
        public void Handle_CallsHandlerHandleWithInputAndContext_AndReturnsOutput()
        {
            // arrange
            var runner = new HandlerRunner<TestHandler, string, string>();
            var handler = A.Fake<TestHandler>();
            var context = A.Fake<ILambdaContext>();

            runner.GetType().GetField("Handler", BindingFlags.NonPublic | BindingFlags.Instance)
                  .SetValue(runner, handler);

            A.CallTo(() => handler.Handle("input", context)).Returns("output");

            // act
            var output = runner.Handle("input", context);

            // assert
            A.CallTo(() => handler.Handle("input", context)).MustHaveHappened();
        }
    }
}
