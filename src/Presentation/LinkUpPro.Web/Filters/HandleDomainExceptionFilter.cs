using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LinkUpPro.Web.Filters;

public class HandleDomainExceptionFilter : IExceptionFilter
{
    private readonly ILogger<HandleDomainExceptionFilter> _logger;

    public HandleDomainExceptionFilter(ILogger<HandleDomainExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is InvalidOperationException)
        {
            _logger.LogWarning(context.Exception, "Regla de negocio violada.");
            
            var factory = context.HttpContext.RequestServices.GetService<ITempDataDictionaryFactory>();
            if (factory != null)
            {
                var tempData = factory.GetTempData(context.HttpContext);
                tempData["Error"] = context.Exception.Message;
            }
            
            context.Result = new RedirectToActionResult("Index", "Home", null);
            context.ExceptionHandled = true;
        }
    }
}
