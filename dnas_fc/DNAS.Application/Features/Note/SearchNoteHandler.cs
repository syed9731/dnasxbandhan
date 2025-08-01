using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.SearchNotes;
using DNAS.Domian.DTO.SearchNotes;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class SearchNoteCommand : IRequest<CommonResponse<SearchNoteData>>
    {
        public FilterSearchNote InputModel { get; set; } = new();
    }
    internal sealed class SearchNoteCommandHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IEncryption encryption, IHttpContextAccessor haccess) : IRequestHandler<SearchNoteCommand, CommonResponse<SearchNoteData>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _encryption= encryption;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<CommonResponse<SearchNoteData>> Handle(SearchNoteCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<SearchNoteData> Response = new();
            try
            {
                ProcGetSearchNoteInput InParams = new()
                {
                    @UserId = Request.InputModel.UserId,
                    @StartDate = Request.InputModel.StartDate ?? "",
                    @EndDate = Request.InputModel.EndDate ?? "",
                    @Category = Request.InputModel.Category ?? "",
                    @Title = Request.InputModel.Title??""
                };

                ProcGetSearchNoteOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<SearchNote, ProcGetSearchNoteOutput>(
                    SpName: OraStoredProcedureNames.ProcGetSearchNote,
                    Params: InParams);
                Response.Data.SearchNotes = DbResult.SearchNotes;
                Response.Data.SearchNotes = Response.Data.SearchNotes.Select(x => { x.NoteId = _encryption.AesEncrypt(x.NoteId); return x; }).ToList();
                return Response;
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during ApprovedNoteCommandHandler------ " + e.Message + Environment.NewLine + e.StackTrace, loginUserId);
                return new CommonResponse<SearchNoteData>();
            }
        }
    }
}
