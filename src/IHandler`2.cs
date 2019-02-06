using Amazon.Lambda.Core;

namespace Lambda.Native
{
    public interface IHandler<in TInput, out TOutput>
    {
        TOutput Handle(TInput input, ILambdaContext context);
    }
}