using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.SendBack;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.DashBoard
{
    public class SendBackCommand : IRequest<CommonResponse<SendBackData>>
    {
        public FilterSendBack InputModel { get; set; } = new();
    }
    internal sealed class SendBackCommandHandler(IDapperFactory IDapperFactory, ICustomLogger logger,IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<SendBackCommand, CommonResponse<SendBackData>>
    {
        private readonly IDapperFactory _IDapperFactory = IDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption= encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<SendBackData>> Handle(SendBackCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<SendBackData> Response = new();
            try
            {
                ProcGetSendBackInput InParams = new()
                {
                    @UserId = Request.InputModel.UserId,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? ""
                };
                ProcGetSendBackOutput DbResult = await _IDapperFactory.ExecuteSpDapperAsync<SendBackTable, ProcGetSendBackOutput>(
                    SpName: OraStoredProcedureNames.ProcGetSendBack,
                    Params: InParams);
                Response.Data.Table = DbResult.Table;
                Response.Data.Table = Response.Data.Table.Select(x => { x.NoteId = _encryption.AesEncrypt(x.NoteId); return x; }).ToList();
                return Response;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during SendBackCommandHandler------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return new CommonResponse<SendBackData>();
            }
        }
    }
}