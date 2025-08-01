using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.NotificationFilter;
using DNAS.Domian.DTO.Draft;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Notification
{
    public class NotificationsCommand : IRequest<CommonResponse<NotificationData>>
    {
        public FilterNotification InputModel { get; set; } = new();
    }
    internal sealed class NotificationsCommandHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<NotificationsCommand, CommonResponse<NotificationData>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<NotificationData>> Handle(NotificationsCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<NotificationData> Response = new();
            try
            {
                ProcNotificationInput InParams = new()
                {
                    @UserId = Request.InputModel.Id,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? "",
                    @NoteStatus = Request.InputModel.Status ?? ""
                };
                ProcNotificationOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<Domian.DTO.Draft.Notification, ProcNotificationOutput>(
                    SpName: OraStoredProcedureNames.ProcNotificationTable,
                    Params: InParams);
                Response.Data.NotificationList = DbResult.NotificationList;
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during NotificationTable------ " + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return new CommonResponse<NotificationData>();
            }
        }
    }
}
