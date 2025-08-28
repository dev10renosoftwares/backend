using System.Net;
using System.Runtime.CompilerServices;

namespace Intern.ServiceModels.BaseServiceModels
{
  
        public class ApiResponse<T>
        {
            public bool Success { get; set; }        
            public string Message { get; set; }      
            public T Data { get; set; }

      
        public HttpStatusCode StatusCode { get; set; }
            public List<string> Errors { get; set; } 

            public ApiResponse()
            {
                Errors = new List<string>();
            }
            public static ApiResponse<T> SuccessResponse(T? data, string message = "Request successful", HttpStatusCode statusCode = HttpStatusCode.OK)
            {
                return new ApiResponse<T>
                {
                    Success = true,
                    Message = message,
                    StatusCode = statusCode,
                    Data = data
                };
            }
            public static ApiResponse<T> FailureResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, T data = default)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = message,
                    StatusCode = statusCode,
                    Data = data
                };
            }

            public static ApiResponse<T> ErrorResponse(string message = "An unexpected error occurred", HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = message,
                    StatusCode = statusCode,
                    Data = default
                };
            }
    }
}


