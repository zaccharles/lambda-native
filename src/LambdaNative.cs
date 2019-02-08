using System;
using System.Net.Http;
using LambdaNative.Internal;

namespace LambdaNative
{
    public static class LambdaNative
    {
        public static void Run<THandler, TInput, TOutput>() where THandler : IHandler<TInput, TOutput>, new()
        {
            IHandlerRunner runner = new HandlerRunner<THandler, TInput, TOutput>();

            Run(runner);
        }

        public static void RunAsync<THandler, TInput, TOutput>() where THandler : IAsyncHandler<TInput, TOutput>, new()
        {
            var uae = Environment.GetEnvironmentVariable("UNWRAP_AGGREGATE_EXCEPTIONS");
            var unwrapAggregateExceptions = uae != null && (uae == "1" || uae.ToLower() == "true");

            IHandlerRunner runner = new AsyncHandlerRunner<THandler, TInput, TOutput>(unwrapAggregateExceptions);

            Run(runner);
        }

        private static void Run(IHandlerRunner runner)
        {
            ILambdaRuntime runtime = new LambdaRuntime(new SystemEnvironment(), new SystemDateTime(), new HttpClient());
            ILambdaBootstrap bootstrap = new LambdaBootstrap(runtime, runner);

            bootstrap.Initialize();
            bootstrap.Run();
        }
    }
}
