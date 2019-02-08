using Amazon.Lambda.Core;

namespace LambdaNative.Internal
{
    internal class AsyncHandlerRunner<THandler, TInput, TOutput> : HandlerRunnerBase<THandler, TInput, TOutput>
        where THandler : IHandler, IAsyncHandler<TInput, TOutput>, new()
    {
        private readonly bool _unwrapAggregateExceptions;

        public AsyncHandlerRunner(bool unwrapAggregateExceptions)
        {
            this.LogDebug($"unwrapAggregateExceptions: {unwrapAggregateExceptions}");

            _unwrapAggregateExceptions = unwrapAggregateExceptions;
        }

        public override TOutput Handle(TInput input, ILambdaContext context)
        {
            this.LogDebug("Calling handler (async)");

            var task = Handler.Handle(input, context);

            return _unwrapAggregateExceptions
                ? task.GetAwaiter().GetResult()
                : task.Result;
        }
    }
}