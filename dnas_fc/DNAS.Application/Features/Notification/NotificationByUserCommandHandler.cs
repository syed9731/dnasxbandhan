using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Draft;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Notification
{
    public class NotificationByUserCommand : IRequest<CommonResponse<HederNotificationsList>>
    {
        public int Usuerid { get; set; }
    }


    public class NotificationByUserCommandHandler(INotificationRep iNotification, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<NotificationByUserCommand, CommonResponse<HederNotificationsList>>
    {
        private readonly INotificationRep _iNotification = iNotification;
        private readonly ICustomLogger _logger=logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<HederNotificationsList>> Handle(NotificationByUserCommand request, CancellationToken cancellationToken)
        {
            try
            {                
                var inparam = new
                {
                    @UserId = request.Usuerid
                };
                CommonResponse<HederNotificationsList>  Response = await _iNotification.HeaderNotificationList(inparam);
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during HeaderNotificationList------ " + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return new CommonResponse<HederNotificationsList>();
            }
        }
    }

}
