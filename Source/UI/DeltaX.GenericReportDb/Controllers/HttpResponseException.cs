using System;
using System.Net;

namespace DeltaX.GenericReportDb.Controllers
{
    public class HttpResponseException : Exception
    {
        public HttpResponseException(HttpStatusCode statusCode)
            : base()
        { StatusCode = statusCode; }

        public HttpResponseException(HttpStatusCode statusCode, string message)
            : base(message)
        { StatusCode = statusCode; }

        public HttpResponseException(HttpStatusCode statusCode, string message, Exception inner)
            : base(message, inner)
        { StatusCode = statusCode; }

        public HttpResponseException(HttpStatusCode statusCode, Exception exception)
            : base(exception.Message, exception.InnerException)
        { StatusCode = statusCode; }

        public HttpStatusCode StatusCode { get; }
    }
}
