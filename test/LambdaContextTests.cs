using System.Collections.Generic;
using LambdaNative.Internal;
using Shouldly;
using Xunit;

namespace LambdaNative.Tests
{
    public class LambdaContextTests
    {
        [Fact]
        public void Constructor_SetsPropertiesUsingEnvironmentVariables()
        {
            // arrange
            var initialEnvironmentVariables = new Dictionary<string, string>
            {
                {"AWS_LAMBDA_FUNCTION_MEMORY_SIZE", "1"},
                {"AWS_LAMBDA_FUNCTION_NAME", "2"},
                {"AWS_LAMBDA_FUNCTION_VERSION", "3"},
                {"AWS_LAMBDA_LOG_GROUP_NAME", "4"},
                {"AWS_LAMBDA_LOG_STREAM_NAME", "5"}
            };

            // act
            var context = new LambdaContext(initialEnvironmentVariables);

            // assert
            context.MemoryLimitInMB.ShouldBe(1);
            context.FunctionName.ShouldBe("2");
            context.FunctionVersion.ShouldBe("3");
            context.LogGroupName.ShouldBe("4");
            context.LogStreamName.ShouldBe("5");
        }
    }
}