using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.FyiFilter;
using DNAS.Domian.DTO.FYI;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.DashBoard
{
    public class FyiCommand : IRequest<CommonResponse<FyiData>>
    {
        public FilterFyi InputModel { get; set; } = new();
    }

    internal sealed class NoteCommandHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess,IEncryption encryption) : IRequestHandler<FyiCommand, CommonResponse<FyiData>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption = encryption;
        private readonly string _loginUserId = $"user_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<CommonResponse<FyiData>> Handle(FyiCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<FyiData> Response = new();
            try
            {
                ProcGetFyiInput InParams = new()
                {
                    @UserId = Request.InputModel.UserId,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? ""
                };
                ProcGetFyiOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<FyiTable, ProcGetFyiOutput>(
                    SpName: OraStoredProcedureNames.ProcGetFYI,
                    Params: InParams);
                Response.Data.Table = DbResult.Table;
                Response.Data.Table = Response.Data.Table.Select(x => { x.noteid = _encryption.AesEncrypt(x.noteid); return x; }).ToList();
                return Response;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during NoteCommandHandler------ " + e.Message + Environment.NewLine + e.StackTrace, _loginUserId);
                return new CommonResponse<FyiData>();
            }
        }
    }
}