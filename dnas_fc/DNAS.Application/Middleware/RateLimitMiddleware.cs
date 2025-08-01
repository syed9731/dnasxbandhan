using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace DNAS.Application.Middleware
{
    public class RateLimitMiddleware(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) : IMiddleware
    {
        private static readonly Dictionary<string, DateTime> RequestTimes = new();
        private readonly IUrlHelperFactory _urlHelperFactory = urlHelperFactory;
        private const int TimeWindowSeconds = 60;
        /// <summary>
        /// Invokes the rate limit middleware.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="next">The next middleware delegate.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Equals("/Note/Create", StringComparison.OrdinalIgnoreCase) && context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {

                if (context.Request.HasFormContentType)
                {
                    // Enable buffering to allow the body to be read multiple times
                    context.Request.EnableBuffering();
                    // Read the form data
                    var form = await context.Request.ReadFormAsync();
                    // Access a specific input field
                    if (form.TryGetValue("NoteId", out var Noteid))
                    {
                        Console.WriteLine("Username: " + Noteid);
                        // Perform any additional processing with the username
                    }
                    // Reset the body stream position so downstream middleware can read it
                    context.Request.Body.Position = 0;

                    var ip = $"{context.Connection.RemoteIpAddress?.ToString()}{context.Request.Path}{Noteid}";
                    if (ip != null)
                    {
                        lock (RequestTimes)
                        {
                            #region Remove outdated entries
                            var filteredRequestTimes = RequestTimes.Where(timestamp => (DateTime.UtcNow - timestamp.Value).TotalSeconds > TimeWindowSeconds)
                                .Select(kvp => kvp.Key).ToList();
                            foreach (var keys in filteredRequestTimes)
                            {
                                RequestTimes.Remove(keys);
                            }
                            #endregion

                            if (RequestTimes.ContainsKey(ip) && (DateTime.Now - RequestTimes[ip]).TotalSeconds < 60)
                            {
                                context.Response.StatusCode = 429;

                                var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
                                    context,
                                    context.GetRouteData(),
                                    new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
                                );

                                var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
                                var targetUrl = urlHelper.Action("", "Error");
                                context.Response.Redirect(targetUrl, permanent: false);
                                return;
                            }
                        }
                        RequestTimes[ip] = DateTime.Now;
                    }
                }
            }
            await next(context);
        }
    }
}
