using Amazon.Lambda.Core;

namespace Lambda.Native
{
    public interface IHandler
    {
        ILambdaSerializer Serializer { get; }
    }
}