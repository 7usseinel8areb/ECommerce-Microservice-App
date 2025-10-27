using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middlewares;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Declare default  variables
        string message = "sorry, internl server error occurred. Kindly try again";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Error";
        try
        {
            await next(context);

            // check if response is too many request // 429 status code
            if (context.Response.StatusCode == (int)HttpStatusCode.TooManyRequests)
            {
                title = "Warning";
                message = "Too many requests. Please try again later.";
                statusCode = (int)HttpStatusCode.TooManyRequests;
                await ModifyHeader(context, title, message, statusCode);
            }

            // check if response is unauthorized // 401 status code
            else if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                title = "Alert";
                message = "You are not authorized to access this resource.";
                statusCode = (int)HttpStatusCode.Unauthorized;
                await ModifyHeader(context, title, message, statusCode);
            }

            // check if response is forbidden // 403 status code
            else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                title = "Out of Access";
                message = "You do not have permission to access this resource.";
                statusCode = (int)HttpStatusCode.Forbidden;
                await ModifyHeader(context, title, message, statusCode);
            }

        }
        catch (Exception ex)
        {
            // Log original exceptions 
            LogException.LogExceptions(ex);

            // Check if exception is timeout exception // 408 status code
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                title = "Out of Time";
                message = "Request timeout .. try again";
                statusCode = (int)HttpStatusCode.RequestTimeout;
            }

            await ModifyHeader(context, title, message, statusCode);
        }
    }

    private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
    {
        // Display scary-free message to client
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Title = title,
            Status = statusCode,
            Detail = message
        }),CancellationToken.None);
        return;
    }
}
