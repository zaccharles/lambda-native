using System;
using System.IO;
using Amazon.Lambda.Core;

namespace LambdaNative.Internal
{
    internal class StreamSerializer : ILambdaSerializer
    {
        public T Deserialize<T>(Stream requestStream)
        {
            return (T)Convert.ChangeType(requestStream, typeof(T));
        }

        public void Serialize<T>(T response, Stream responseStream)
        {
            ((Stream)(object)response).CopyTo(responseStream);
        }
    }
}