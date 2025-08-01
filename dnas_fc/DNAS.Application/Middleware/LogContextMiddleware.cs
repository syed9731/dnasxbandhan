using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System.Security.Claims;

namespace DNAS.Application.Middleware
{
    public class LogContextMiddleware(IHttpContextAccessor haccess) : IMiddleware
    {        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var custId = haccess.HttpContext?.User.FindFirstValue("UserId") ?? "anonymous";            
            using (LogContext.PushProperty("CustId", custId))
            {
                await next(context);
            }
        }
    }
}
