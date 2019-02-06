using System.IO;
using Amazon.Lambda.Core;

namespace LambdaNative.Internal
{
    internal interface IHandlerRunner
    {
        void Initialize();
        Stream Handle(Stream inputStream, ILambdaContext context);
    }
}