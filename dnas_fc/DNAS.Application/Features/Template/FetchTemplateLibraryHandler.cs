using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.TemplateLibrary;
using DNAS.Domian.DTO.Template;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Login
{
    public class TemplateLibraryCommand : IRequest<CommonResponse<TemplateModelData>>
    {
        public FilterTemplateModel InputModel { get; set; } = new();
    }
    internal sealed class FetchTemplateLibraryHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IEncryption enc, IHttpContextAccessor haccess) : IRequestHandler<TemplateLibraryCommand, CommonResponse<TemplateModelData>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<TemplateModelData>> Handle(TemplateLibraryCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<TemplateModelData> Response = new();
            try
            {
                ProcFetchTemplateInput InParams = new()
                {
                    @UserId = Request.InputModel.UserId,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? ""
                };
                ProcFetchTemplateOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<TemplateModel, ProcFetchTemplateOutput>(
                    SpName: OraStoredProcedureNames.ProcFetchTemplate,
                    Params: InParams);
                Response.Data.TemplateList = DbResult.TemplateList;

                if (Response.Data.TemplateList != null)
                {
                    Response.Data.TemplateList = Response.Data.TemplateList.Select(x => { x.TemplateId = _encryption.AesEncrypt(x.TemplateId); return x; }).ToList();
                    _logger.LogwriteInfo("Template List Fetch successfully", loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo("No Data Found in the Table", loginUserId);
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during TemplateLibraryCommand execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return Response;
            }
        }
    }
}