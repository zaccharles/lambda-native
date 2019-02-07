using System.Net.Http;
using LambdaNative.Internal;

namespace LambdaNative
{
    public class EntryPoint<THandler, TInput, TOutput> where THandler : IHandler, IHandler<TInput, TOutput>, new()
    {
        public static void Main(string[] args)
        {
            ILambdaRuntime runtime = new LambdaRuntime(new SystemEnvironment(), new SystemDateTime(), new HttpClient());
            IHandlerRunner runner = new HandlerRunner<THandler, TInput, TOutput>();
            ILambdaBootstrap bootstrap = new LambdaBootstrap(runtime, runner);

            bootstrap.Initialize();
            bootstrap.Run();
        }
    }
}
