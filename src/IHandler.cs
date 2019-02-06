using Amazon.Lambda.Core;

namespace LambdaNative
{
    public interface IHandler
    {
        ILambdaSerializer Serializer { get; }
    }
}