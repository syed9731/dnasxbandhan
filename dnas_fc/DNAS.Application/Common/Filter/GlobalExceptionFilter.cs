using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DNAS.Application.Common.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {            
            _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);
            context.Result = new RedirectToActionResult("Error", "Error", null);
            context.ExceptionHandled = true;
        }
    }
}
