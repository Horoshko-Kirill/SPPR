using Serilog;

namespace WEB_353505_Horoshko.HelperClasses
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Log.ForContext<LoggingMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBody = context.Response.Body;

            try
            {
                await _next(context);

                if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
                {
                    _logger.Information("---> request {Url} returns {StatusCode}",
                        context.Request.Path,
                        context.Response.StatusCode);
                }
            }
            finally
            {
                context.Response.Body = originalBody;
            }
        }
    }
}
