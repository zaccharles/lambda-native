using Amazon.Lambda.Core;

namespace LambdaNative.Internal
{
    internal class HandlerRunner<THandler, TInput, TOutput> : HandlerRunnerBase<THandler, TInput, TOutput>
        where THandler : IHandler, IHandler<TInput, TOutput>, new()
    {
        public override TOutput Handle(TInput input, ILambdaContext context)
        {
            this.LogDebug("Calling handler");
            return Handler.Handle(input, context);
        }
    }
}