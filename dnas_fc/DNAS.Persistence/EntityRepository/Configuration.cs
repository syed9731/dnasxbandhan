using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domain.DAO.DbHelperModels.FetchConfiguration;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.Confguration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
    internal class Configuration(ICustomLogger logger, IDapperFactory iDapperFactory, IHttpContextAccessor haccess) : IConfiguration
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        private readonly ICustomLogger _logger=logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<ConfigurationRespModel>> FetchConfiguration(object inparam)
        {
            CommonResponse<ConfigurationRespModel> Response = new();
            try
            {
                ConfigurationRespModel DbResponse = await _iDapperFactory.ExecuteSpDapperAsync<ConfigurationResp, ConfigurationRespModel>
                    (SpName: OraStoredProcedureNames.ProcFetchConfiguration, inparam);
                Response.Data = DbResponse;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during Configuration command execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace,string.IsNullOrEmpty(haccess.HttpContext?.User.FindFirstValue("UserId"))? "Login": loginUserId );
            }

            return Response;
        }
    }
}
