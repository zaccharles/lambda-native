using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Lambda.Native
{
    public interface IAsyncHandler<in TInput, TOutput>
    {
        Task<TOutput> Handle(TInput input, ILambdaContext context);
    }
}