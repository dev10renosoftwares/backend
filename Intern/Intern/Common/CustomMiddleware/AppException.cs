using System;
using System.Net;

namespace Common.Helpers  // Make sure this namespace matches your project
{
    public class AppException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public AppException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
