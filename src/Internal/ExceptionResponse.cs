using System;
using System.Linq;

namespace LambdaNative.Internal
{
    public class ExceptionResponse
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string[] StackTrace { get; set; }
        public ExceptionResponse InnerException { get; set; }
        public ExceptionResponse[] InnerExceptions { get; set; }

        public static ExceptionResponse Create(Exception ex)
        {
            var response = new ExceptionResponse();

            if (ex != null)
            {
                response.ErrorType = ex.GetType().Name;
                response.ErrorMessage = ex.Message;
                response.StackTrace = ex.StackTrace?.Split('\n').ToArray();
                response.InnerException = ex.InnerException != null ? Create(ex) : null;

                if (ex is AggregateException aggregateException)
                {
                    response.InnerExceptions =
                        aggregateException.InnerExceptions?.Select(x => Create(x)).ToArray();
                }
            }

            return response;
        }
    }
}