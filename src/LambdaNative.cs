using System;
using System.Net.Http;
using LambdaNative.Internal;

namespace LambdaNative
{
    public static class LambdaNative
    {
        public static void Run<THandler, TInput, TOutput>() where THandler : IHandler<TInput, TOutput>, new()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleException;

            IHandlerRunner runner = new HandlerRunner<THandler, TInput, TOutput>();

            Run(runner);
        }

        public static void RunAsync<THandler, TInput, TOutput>() where THandler : IAsyncHandler<TInput, TOutput>, new()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleException;

            var uae = Environment.GetEnvironmentVariable("UNWRAP_AGGREGATE_EXCEPTIONS");
            var unwrapAggregateExceptions = uae != null && (uae == "1" || uae.ToLower() == "true");

            IHandlerRunner runner = new AsyncHandlerRunner<THandler, TInput, TOutput>(unwrapAggregateExceptions);

            Run(runner);
        }

        private static void Run(IHandlerRunner runner)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(30);

            ILambdaRuntime runtime = new LambdaRuntime(new SystemEnvironment(), new SystemDateTime(), httpClient);
            ILambdaBootstrap bootstrap = new LambdaBootstrap(runtime, runner);

            bootstrap.Initialize();
            bootstrap.Run();
        }

        private static void HandleException(object sender, UnhandledExceptionEventArgs args)
        {
            var ex = (Exception)args.ExceptionObject;
            LogException(ex);
        }

        private static void LogException(Exception ex, bool isInnerException = false)
        {
            if (isInnerException)
            {
                typeof(LambdaNative).LogDebug("---------------------------------------------------");
                typeof(LambdaNative).LogDebug($"Inner exception: {ex.GetType().Name}: {ex.Message}");
            }
            else
            {
                typeof(LambdaNative).LogDebug("===================================================");
                typeof(LambdaNative).LogDebug($"{ex.GetType().Name} : {ex.Message}");
            }

            typeof(LambdaNative).LogDebug($"{ex.StackTrace}");

            if (ex.InnerException != null) LogException(ex.InnerException, true);
            if (!(ex is AggregateException aggregateException)) return;

            foreach (var iex in aggregateException.InnerExceptions)
            {
                LogException(iex, true);
            }
        }
    }
}
