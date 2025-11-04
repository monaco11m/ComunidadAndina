using System.Net;
using Serilog;
using System.Text.Json;
using OrderManagement.Application.Exceptions;

namespace OrderManagement.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode;
            object errorResponse;

            switch (ex)
            {
                case NotFoundException notFound:
                    statusCode = HttpStatusCode.NotFound;
                    errorResponse = new { message = notFound.Message };
                    break;

                case ValidationException validation:
                    statusCode = HttpStatusCode.BadRequest;
                    errorResponse = new { message = validation.Message, errors = validation.Errors };
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    errorResponse = new { message = "An unexpected error occurred." };
                    break;
            }

            Log.Error(ex, "Exception handled by middleware");

            context.Response.StatusCode = (int)statusCode;
            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }

    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
