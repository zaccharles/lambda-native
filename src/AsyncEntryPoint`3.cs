using System;
using System.Net.Http;
using LambdaNative.Internal;

namespace LambdaNative
{
    public class AsyncEntryPoint<THandler, TInput, TOutput> where THandler : IHandler, IAsyncHandler<TInput, TOutput>, new()
    {
        public static void Main(string[] args)
        {
            var uae = Environment.GetEnvironmentVariable("UNWRAP_AGGREGATE_EXCEPTIONS");
            var unwrapAggregateExceptions = uae != null && (uae == "1" || uae.ToLower() == "true");

            ILambdaRuntime runtime = new LambdaRuntime(new SystemEnvironment(), new SystemDateTime(), new HttpClient());
            IHandlerRunner runner = new AsyncHandlerRunner<THandler, TInput, TOutput>(unwrapAggregateExceptions);
            ILambdaBootstrap bootstrap = new LambdaBootstrap(runtime, runner);

            bootstrap.Initialize();
            bootstrap.Run();
        }
    }
}