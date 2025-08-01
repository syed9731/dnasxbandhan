using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DAO.DbHelperModels.HeaderNotificationList;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Draft;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
    internal class NotificationRep(ICustomLogger iCustomLogger, IDapperFactory iDapperFactory, IHttpContextAccessor haccess) : INotificationRep
    {
        private readonly ICustomLogger _iCustomLogger = iCustomLogger;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<HederNotificationsList>> HeaderNotificationList(object inparam)
        {
            CommonResponse<HederNotificationsList> Response = new();
            try
            {
                ProcGetNotificationByUserOutput DbResponse = await _iDapperFactory.ExecuteSpDapperAsync<HederNotificationsList, ProcGetNotificationByUserOutput>
                    (SpName: OraStoredProcedureNames.ProcGetNotificationByUser, inparam);
                Response.Data = DbResponse.HederNotifications;
            }
            catch (Exception e)
            {
                _iCustomLogger.LogwriteInfo("exception occur during NotificationTable------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
            }
            return Response;
        }
    }
}
