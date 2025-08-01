using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNAS.Application.Middleware
{
    public class BlockBurpSuiteMiddleware:IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            if (userAgent.Contains("Burp", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Burp Suite usage detected and blocked.");
                return;
            }

            await next(context);
        }
    }
}
