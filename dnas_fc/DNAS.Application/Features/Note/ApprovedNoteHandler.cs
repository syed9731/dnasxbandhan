using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.ApprovedNotes;
using DNAS.Domian.DTO.ApprovedNotes;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace DNAS.Application.Features.Note
{
    public class ApprovedNoteCommand : IRequest<CommonResponse<ApprovedNoteData>>
    {
        public FilterApprovedNote InputModel { get; set; } = new();
    }
    internal sealed class ApprovedNoteCommandHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess,IEncryption encryption) : IRequestHandler<ApprovedNoteCommand, CommonResponse<ApprovedNoteData>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption= encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<ApprovedNoteData>> Handle(ApprovedNoteCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<ApprovedNoteData> Response = new();
            try
            {
                ProcGetApprovedNoteInput InParams = new()
                {
                    @UserId = Request.InputModel.UserId,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? ""
                };
                _logger.LogwriteInfo("before call ProcGetApprovedNote procedure with parameters------ " + JsonSerializer.Serialize(InParams), loginUserId);
                ProcGetApprovedNoteOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<ApprovedNote, ProcGetApprovedNoteOutput>(
                    SpName: OraStoredProcedureNames.ProcGetApprovedNote,
                    Params: InParams);
                Response.Data.Table = DbResult.Table;
                Response.Data.Table = Response.Data.Table.Select(x => { x.NoteId = _encryption.AesEncrypt(x.NoteId); return x; }).ToList();
                _logger.LogwriteInfo("after call ProcGetApprovedNote procedure with return value------ " + JsonSerializer.Serialize(Response.Data.Table), loginUserId);
                foreach (ApprovedNote Row in Response.Data.Table)
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
                _logger.LogwriteInfo("exception occur during ApprovedNoteCommandHandler------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return new CommonResponse<ApprovedNoteData>();
            }
        }
    }
}
