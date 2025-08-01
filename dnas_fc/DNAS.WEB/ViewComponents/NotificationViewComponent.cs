using DNAS.Application.Features.Notification;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DNAS.Domian.DTO.Draft;
using DNAS.Domian.Common;
using MediatR;

namespace DNAS.WEB.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly ISender _sender;
        private readonly HttpContext hcontext;
        public NotificationViewComponent(ISender sender, IHttpContextAccessor haccess)
        {            
            _sender = sender;
            hcontext = haccess.HttpContext!;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal cp = hcontext.User;
            NotificationByUserCommand input = new()
            {
                Usuerid = Convert.ToInt32(cp.FindFirstValue("UserId")),
            };
            CommonResponse<HederNotificationsList> response = await _sender.Send(input);
            return View(response.Data);
        }
    }
}
