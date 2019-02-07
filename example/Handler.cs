using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;

namespace LambdaNative.Example
{
    public class Handler : IHandler<Request, Response>
    {
        public ILambdaSerializer Serializer => new JsonSerializer();

        public Response Handle(Request request, ILambdaContext context)
        {
            return new Response("Lambda Native! Your function executed successfully!", request);
        }
    }

    public class Request
    {
        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }

        public Request(string key1, string key2, string key3)
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
        }
    }

    public class Response
    {
        public string Message { get; set; }
        public Request Request { get; set; }

        public Response(string message, Request request)
        {
            Message = message;
            Request = request;
        }
    }
}
