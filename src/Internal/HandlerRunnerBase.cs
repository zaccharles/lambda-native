using System;
using System.IO;
using Amazon.Lambda.Core;

namespace LambdaNative.Internal
{
    internal abstract class HandlerRunnerBase<THandler, TInput, TOutput> : IHandlerRunner
        where THandler : IHandler, new()
    {
        protected THandler Handler;
        private ILambdaSerializer _serializer;

        public void Initialize()
        {
            Handler = new THandler();
            _serializer = Handler.Serializer;

            if (_serializer != null) return;

            if (typeof(TInput) != typeof(Stream) || typeof(TOutput) != typeof(Stream))
            {
                throw new LambdaValidationException(
                    $"'Serializer' property not set on handler '{Handler.GetType().FullName}'. " +
                    "To use types other than System.IO.Stream as input/output parameters, " +
                    "the 'Serializer' property must be set.");
            }

            _serializer = new StreamSerializer();
        }

        public abstract TOutput Handle(TInput input, ILambdaContext context);

        public Stream Handle(Stream inputStream, ILambdaContext context)
        {
            TInput input;
            TOutput output;

            try
            {
                input = _serializer.Deserialize<TInput>(inputStream);
            }
            catch (Exception ex)
            {
                throw new LambdaUserCodeException("An exception was thrown while deserializing the input.", ex);
            }

            try
            {

                output = Handle(input, context);
            }
            catch (Exception ex)
            {
                throw new LambdaUserCodeException("An exception was thrown by the Handle method.", ex);
            }

            try
            {
                var outputStream = new MemoryStream();
                _serializer.Serialize(output, outputStream);
                return outputStream;
            }
            catch (Exception ex)
            {
                throw new LambdaUserCodeException("An exception was thrown while serializing the output.", ex);
            }
        }
    }
}