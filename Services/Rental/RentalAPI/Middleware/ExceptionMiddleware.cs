using Shared.Exceptions;

namespace RentalAPI.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex.Message, StatusCodes.Status404NotFound);
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex.Message, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, "Something went wrong", StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var result = System.Text.Json.JsonSerializer.Serialize(new { message });
            await context.Response.WriteAsync(result);
        }
    }

}
