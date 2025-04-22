
using System.Text.Json;

namespace ToDoApp.API
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext); // تمرير الطلب إلى المكونات التالية في pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred."); // تسجيل الخطأ
                await HandleExceptionAsync(httpContext, ex); // معالجة الاستثناء
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string message = "An unexpected error occurred.";

            if (exception is ApplicationException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
            }

            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new
            {
                StatusCode = statusCode,
                Message = message
            });

            return context.Response.WriteAsync(result);
        }
    }

}
