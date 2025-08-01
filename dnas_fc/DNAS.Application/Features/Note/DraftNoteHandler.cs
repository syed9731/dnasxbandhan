using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.DraftNotes;
using DNAS.Domian.DTO.Draft;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Note
{
    public class DraftNoteCommand : IRequest<CommonResponse<DraftNoteData>>
    {
        public FilterDraftNote FilterDraftNotes { get; set; } = new();
    }

    internal sealed class DraftNoteCommandHandler(IDapperFactory iDapperFactory, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<DraftNoteCommand, CommonResponse<DraftNoteData>>
    {
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<CommonResponse<DraftNoteData>> Handle(DraftNoteCommand Request, CancellationToken cancellationToken)
        {
            CommonResponse<DraftNoteData> Response = new();
            try
            {
                ProcFetchDraftListInput InParams = new()
                {
                    @UserId = Request.FilterDraftNotes.UserId,
                    @StartDate = Request.FilterDraftNotes.StartDate ?? "",
                    @EndDate = Request.FilterDraftNotes.EndDate ?? "",
                    @Category = Request.FilterDraftNotes.Category ?? ""
                };
                ProcFetchDraftListOutput DbResult = await _iDapperFactory.ExecuteSpDapperAsync<DraftNote, ProcFetchDraftListOutput>(
                    SpName: OraStoredProcedureNames.ProcFetchDraftList,
                    Params: InParams);
                Response.Data.DraftNotes = DbResult.DraftNotes;

                if (Response.Data != null)
                {
                    _logger.LogwriteInfo("Draft Note fetch successfully", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("Draft Note fetch failed", loginUserId);
                    return new CommonResponse<DraftNoteData>();
                }                
            }
            catch (Exception ex)
            {
                _logger.LogwriteError(ex.ToString(), loginUserId);
                return new CommonResponse<DraftNoteData>();
            }
        }
    }
}
