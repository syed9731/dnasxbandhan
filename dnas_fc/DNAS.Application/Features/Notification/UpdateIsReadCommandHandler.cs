using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.NotificationIsRead;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Notification
{
    public class UpdateIsReadCommand : IRequest<CommonResponse<Domian.DTO.Draft.Notification>>
    {
        public int NotificationId { get; set; } = 0;
    }

    internal sealed class UpdateIsReadCommandHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<UpdateIsReadCommand, CommonResponse<Domian.DTO.Draft.Notification>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<Domian.DTO.Draft.Notification>> Handle(UpdateIsReadCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<Domian.DTO.Draft.Notification> Response = new();
            try
            {
                ProcNotificationIsReadInput InParams = new()
                {
                    @NotificationId = Request.NotificationId,
                };
                ProcNotificationIsReadOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<Domian.DTO.Draft.Notification, ProcNotificationIsReadOutput>(
                    SpName: OraStoredProcedureNames.ProcNotificationIsRead,
                    Params: InParams);
                Response.Data = DbResult.NotificationReadStatus;
                return Response;
            }
            catch (Exception ex)
            {                
                _logger.LogwriteInfo("exception occur during UpdateIsReadCommand execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return new CommonResponse<Domian.DTO.Draft.Notification>();
            }
        }
    }
}