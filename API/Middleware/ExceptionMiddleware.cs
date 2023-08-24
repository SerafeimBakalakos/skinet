using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
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
                int errorCode = (int)HttpStatusCode.InternalServerError;   

                _logger.LogError(ex, ex.Message);

                // Write our response
                var response = _env.IsDevelopment() 
                    ? new ApiException(errorCode, ex.Message, ex.StackTrace.ToString()) 
                    : new ApiException(errorCode);
                // StackTrace.ToString() is an extension method that formats the original string

                // Convert our response to JSON format. 
                // Outside an API controller, it will not automatically be formatted to the camel-case convention for JSON
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                // Pass our response to the http context in JSON format.
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = errorCode;
                await context.Response.WriteAsync(json);
            }
        }
    }
}