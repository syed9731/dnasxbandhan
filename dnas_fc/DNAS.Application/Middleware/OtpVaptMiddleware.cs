using Microsoft.AspNetCore.Http;

using System.Text;

namespace DNAS.Application.Middleware
{
    public class OtpVaptMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //Check if the request is for OTP validation
            if (context.Request.Path.StartsWithSegments("/Login/Otp"))
            {
                //await context.Session.LoadAsync();
                string? otpVerified = context.Session.GetString("GeneratedOTP");

                //If OTP is missing or invalid, redirect to login page
                if (string.IsNullOrWhiteSpace(otpVerified))
                {
                    context.Response.Redirect("/Login/Logout");
                    return;
                }
            }

            //If OTP is valid, continue request processing
            await next(context);
        }
    }
}
