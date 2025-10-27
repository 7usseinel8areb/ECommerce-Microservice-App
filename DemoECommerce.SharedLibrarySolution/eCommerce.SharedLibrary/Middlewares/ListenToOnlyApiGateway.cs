using Microsoft.AspNetCore.Http;
using System.Net;

namespace eCommerce.SharedLibrary.Middlewares
{
    public class ListenToOnlyApiGateway(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Extract a specific header from the request
            var signedHeader = context.Request.Headers["Api-Gateway"];

            // Null means the request is no coming from the api gateway // 503
            if(signedHeader.FirstOrDefault() is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await context.Response.WriteAsync("Sorry, service is unavailable.");
                return;
            }

            await next(context);
        }
    }
}
