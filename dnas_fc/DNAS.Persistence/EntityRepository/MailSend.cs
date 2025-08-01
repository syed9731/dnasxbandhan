using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Confguration;
using DNAS.Persistence.SqlFactory;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
    internal class MailSend(ISqlDapper iSqlDapper, IHttpContextAccessor haccess, ICustomLogger logger) : IMailSend
    {
        private readonly ISqlDapper _iSqlDapper = iSqlDapper;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public readonly ICustomLogger _logger = logger;
        public async Task<IEnumerable<ConfigurationRespModel>> fetchConfiguration(object inparam)
        {
            try
            {
                return await _iSqlDapper.GetAll<ConfigurationRespModel>(OraStoredProcedureNames.ProcFetchConfiguration, inparam);
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during fetchConfiguration execution-------" + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return [];
            }             
        }
    }
}
