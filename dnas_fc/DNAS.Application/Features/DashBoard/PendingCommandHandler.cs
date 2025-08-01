using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.PendingNote;
using DNAS.Domian.DTO.Note;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.DashBoard
{
    public class PendingCommand : IRequest<CommonResponse<PendingData>>
    {
        public FilterPendingNote InputModel { get; set; } = new();
    }

    internal sealed class FetchWidthdrawListHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess, IEncryption encryption) : IRequestHandler<PendingCommand, CommonResponse<PendingData>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption= encryption;
        private readonly string _loginUserId = $"user_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<CommonResponse<PendingData>> Handle(PendingCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<PendingData> Response = new();
            try
            {
                ProcGetPendingInput InParams = new()
                {
                    @UserId = Request.InputModel.UserId,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? ""
                };

                ProcGetPendingOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<PendingTable, ProcGetPendingOutput>(
                    SpName: OraStoredProcedureNames.ProcGetPending,
                    Params: InParams);
                Response.Data.Table = DbResult.Table;
                Response.Data.Table = Response.Data.Table.Select(x => { x.NoteId = _encryption.AesEncrypt(x.NoteId); return x; }).ToList();
                foreach (PendingTable Row in Response.Data.Table)
                {
                    if (!string.IsNullOrEmpty(Row.DateOfCreation))
                    {
                        int Age = (DateTime.UtcNow - Convert.ToDateTime(Row.DateOfCreation)).Days;
                        if (Age > 1) Row.Aging = Age.ToString() + " Days";
                        else Row.Aging = Age.ToString() + " Day";
                    }
                    else { Row.Aging = "0 Day"; }
                }
                return Response;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during PendingCommandHandler------ " + e.Message + Environment.NewLine + e.StackTrace, _loginUserId);
                return new CommonResponse<PendingData>();
            }
        }
    }
}
