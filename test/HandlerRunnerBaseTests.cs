using Amazon.Lambda.Core;
using FakeItEasy;
using LambdaNative.Internal;
using Shouldly;
using System;
using System.IO;
using System.Reflection;
using Xunit;
using LambdaNative.Tests.Extensions;

namespace LambdaNative.Tests
{
    public class HandlerRunnerBaseTests
    {
        public class TestHandler : IHandler, IHandler<string, string>
        {
            public virtual ILambdaSerializer Serializer { get; }

            public virtual string Handle(string input, ILambdaContext context)
            {
                throw new NotImplementedException();
            }
        }

        internal class GenericTestHandlerRunner<TInput, TOutput> : HandlerRunnerBase<TestHandler, TInput, TOutput>
        {
            public override TOutput Handle(TInput input, ILambdaContext context)
            {
                throw new NotImplementedException();
            }
        }

        internal class TestHandlerRunner : HandlerRunnerBase<TestHandler, string, string>
        {
            public override string Handle(string input, ILambdaContext context)
            {
                return Handler.Handle(input, context);
            }
        }

        [Fact]
        public void InitializeWithoutSerializer_ThrowsException_WhenInputTypeIsNotStream()
        {
            // arrange
            var runner = new GenericTestHandlerRunner<string, Stream>();

            // act
            var ex = Record.Exception(() => runner.Initialize());

            // assert
            ex.ShouldBeOfType<LambdaValidationException>();
        }

        [Fact]
        public void InitializeWithoutSerializer_ThrowsException_WhenOutputTypeIsNotStream()
        {
            // arrange
            var runner = new GenericTestHandlerRunner<Stream, string>();

            // act
            var ex = Record.Exception(() => runner.Initialize());

            // assert
            ex.ShouldBeOfType<LambdaValidationException>();
        }

        [Fact]
        public void InitializeWithoutSerializer_DoesNotThrowException_WhenInputAndOutputTypeBothStream()
        {
            // arrange
            var runner = new GenericTestHandlerRunner<Stream, Stream>();

            // act
            var ex = Record.Exception(() => runner.Initialize());

            // assert
            ex.ShouldBeNull();
        }

        [Fact]
        public void Handle_DeserializesInput_CallsHandler_AndSerializesOutput()
        {
            // arrange
            var runner = new TestHandlerRunner();
            var handler = A.Fake<TestHandler>();
            var serializer = A.Fake<ILambdaSerializer>();
            var context = A.Fake<ILambdaContext>();

            runner.GetType().GetField("Handler", BindingFlags.NonPublic | BindingFlags.Instance)
                  .SetValue(runner, handler);

            runner.GetType().BaseType.GetField("_serializer", BindingFlags.NonPublic | BindingFlags.Instance)
                  .SetValue(runner, serializer);

            var inputStream = new MemoryStream();
            var outputStream = new MemoryStream();

            A.CallTo(() => serializer.Deserialize<string>(inputStream)).Returns("input");
            A.CallTo(() => handler.Handle("input", context)).Returns("output");
            A.CallTo(() => serializer.Serialize("output", A<Stream>.Ignored))
                .Invokes(c => { "output".ToStream().CopyTo((Stream)c.Arguments[1]); });
                
            // act
            var output = runner.Handle(inputStream, context);

            // assert
            output.FromStream().ShouldBe("output");
        }
    }
}
