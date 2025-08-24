using Intern.ServiceModels.BaseServiceModels;
using System.Net;
using System.Text.Json;
using Common.Helpers;




namespace Intern.Common.CustomMiddleware;



public class ExceptionMiddleware
{

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status = HttpStatusCode.InternalServerError;
        string message = "Something went wrong.";

        if (exception is AppException appEx)
        {
            status = appEx.StatusCode;
            message = appEx.Message;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var response = new ApiResponse<string>
        {
            Success = false,
            
            Message = message,

            StatusCode = status
        };

        var result = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(result);
    }
}
