using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.TemplateLibrary;
using DNAS.Domian.DTO.Template;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Persistence.Repository
{
    internal class Template(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess) : ITemplate
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<TemplateModel> ViewTemplate(object inparam)
        {
            try
            {
                CommonResponse<TemplateModelData> Response = new();
                ProcFetchTemplateOutput DbResponse = await _iDapperFactory.ExecuteSpDapperAsync<TemplateModel, ProcFetchTemplateOutput>
                    (SpName: OraStoredProcedureNames.ProcFetchTemplateByTemplateId, Params: inparam);

                Response.Data.TemplateList = DbResponse.TemplateList;
                return Response.Data.TemplateList.FirstOrDefault() ?? new();
            }
            catch (Exception Ex)
            {
                _logger.LogwriteInfo("exception occur during ViewTemplate------ " + Ex.Message + Environment.NewLine + Ex.StackTrace, loginUserId);
                return new();
            }
        }
    }
}
