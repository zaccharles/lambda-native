using Amazon.Lambda.Core;

namespace LambdaNative
{
    public interface IHandler<in TInput, out TOutput>
    {
        TOutput Handle(TInput input, ILambdaContext context);
    }
}