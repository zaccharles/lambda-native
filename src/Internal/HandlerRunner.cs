using Amazon.Lambda.Core;

namespace Lambda.Native.Internal
{
    internal class HandlerRunner<THandler, TInput, TOutput> : HandlerRunnerBase<THandler, TInput, TOutput>
        where THandler : IHandler, IHandler<TInput, TOutput>, new()
    {
        public override TOutput Handle(TInput input, ILambdaContext context)
        {
            return Handler.Handle(input, context);
        }
    }
}