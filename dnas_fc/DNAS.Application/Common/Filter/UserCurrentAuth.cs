using DNAS.Application.IRepository;
using DNAS.Domain.DTO.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DNAS.Application.Common.Filter
{
    public class UserCurrentAuth(ILogin iLogin, IHttpContextAccessor haccess) : IAsyncActionFilter//Attribute, IAuthorizationFilter
    {

        private readonly ILogin _iLogin = iLogin;
        private readonly string loginUserId = haccess.HttpContext?.User.FindFirstValue("UserId")!;
        private readonly string SessionID = haccess.HttpContext?.User.FindFirstValue("SessionId")!;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {            
            var inparam = new
            {
                @UserId = loginUserId,
            };
            UserTrackingModel Response = await _iLogin.CheckUsertracking(inparam);

            if (Response.SessionId != SessionID)
            {
                context.Result = new RedirectToActionResult("LogoutCurrentAuth", "login", null);
            }
            else
            {
                var userPrincipal = haccess.HttpContext?.User;

                if (userPrincipal?.Identity?.IsAuthenticated == true)
                {
                    var authProperties = new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(30),
                        IsPersistent = true
                    };
                    await haccess.HttpContext!.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        userPrincipal,
                        authProperties);
                }
                await next();
            }
        }
    }
}
