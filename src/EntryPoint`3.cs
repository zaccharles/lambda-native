using Lambda.Native.Internal;

namespace Lambda.Native
{
    public class EntryPoint<THandler, TInput, TOutput> where THandler : IHandler, IHandler<TInput, TOutput>, new()
    {
        public static void Main(string[] args)
        {
            ILambdaRuntime runtime = new LambdaRuntime(new SystemEnvironment());
            IHandlerRunner runner = new HandlerRunner<THandler, TInput, TOutput>();
            ILambdaBootstrap bootstrap = new LambdaBootstrap(runtime, runner);

            bootstrap.Initialize();
            bootstrap.Run();
        }
    }
}
