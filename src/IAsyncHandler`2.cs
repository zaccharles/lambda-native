using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace LambdaNative
{
    public interface IAsyncHandler<in TInput, TOutput> : IHandler
    {
        Task<TOutput> Handle(TInput input, ILambdaContext context);
    }
}