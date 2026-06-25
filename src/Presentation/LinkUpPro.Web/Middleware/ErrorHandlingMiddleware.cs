using System.Net;

namespace LinkUpPro.Web.Middleware;

/// <summary>
/// Middleware for centralized error handling.
/// Catches unhandled exceptions and returns appropriate error responses.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error no controlado en la solicitud {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "text/html; charset=utf-8";

        // In development, show detailed error information
        if (_env.IsDevelopment())
        {
            var errorHtml = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Error</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; padding: 20px; }}
                        h1 {{ color: #d9534f; }}
                        pre {{ background-color: #f5f5f5; padding: 15px; border-radius: 5px; overflow-x: auto; }}
                    </style>
                </head>
                <body>
                    <h1>⚠️ Error de Desarrollo</h1>
                    <p><strong>Mensaje:</strong> {System.Net.WebUtility.HtmlEncode(exception.Message)}</p>
                    <p><strong>Tipo:</strong> {System.Net.WebUtility.HtmlEncode(exception.GetType().FullName)}</p>
                    <h3>Stack Trace:</h3>
                    <pre>{System.Net.WebUtility.HtmlEncode(exception.StackTrace)}</pre>
                </body>
                </html>";

            return context.Response.WriteAsync(errorHtml);
        }
        else
        {
            // In production, show a generic error message
            var errorHtml = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Error</title>
                    <style>
                        body {{ font-family: Arial, sans-serif; padding: 20px; text-align: center; }}
                        h1 {{ color: #d9534f; }}
                    </style>
                </head>
                <body>
                    <h1>⚠️ Error</h1>
                    <p>Ha ocurrido un error inesperado. Por favor, intente nuevamente más tarde.</p>
                    <p><a href=""/"">Volver al inicio</a></p>
                </body>
                </html>";

            return context.Response.WriteAsync(errorHtml);
        }
    }
}
