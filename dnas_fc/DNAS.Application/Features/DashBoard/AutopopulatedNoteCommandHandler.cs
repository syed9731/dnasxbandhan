using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domain.DAO.DbHelperModels.AutopopulatedNotes;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace DNAS.Application.Features.DashBoard
{
    public record AutopopulatedNoteCommand : IRequest<CommonResponse<AutopopulatedNotes>>
    {
        public FilterNote FilterData { get; set; } = new();
    }
    internal sealed class AutopopulatedNoteCommandHandler(IDapperFactory dapperFactory, IHttpContextAccessor httpContextAccessor, ICustomLogger customLogger, IEncryption encryption) : IRequestHandler<AutopopulatedNoteCommand, CommonResponse<AutopopulatedNotes>>
    {
        private readonly IDapperFactory _dapperFactory = dapperFactory;
        private readonly ICustomLogger _logger = customLogger;
        private readonly IEncryption _encryption = encryption;
        private readonly string _loginUserId = $"user_{httpContextAccessor.HttpContext?.User.FindFirst("UserId")}";

        public async Task<CommonResponse<AutopopulatedNotes>> Handle(AutopopulatedNoteCommand request, CancellationToken cancellationToken)
        {
            CommonResponse<AutopopulatedNotes> response = new();

            try
            {
                //set SqlParameter for stored procedure
                ProcFetchAutopopulatedNoteInput procParams = new()
                {
                    @UserId = request.FilterData.UserId,
                    @SearchKey = request.FilterData.SearchText,
                    @DashboardType=request.FilterData.DashboardType
                };

                //call the stored procedure
                ProcFetchAutopopulatedNoteOutput dbResult = await _dapperFactory.ExecuteSpDapperAsync<NoteData, ProcFetchAutopopulatedNoteOutput>(
                    SpName: OraStoredProcedureNames.ProcSearchApproverNote,
                    Params: procParams);

                //assign stored procedure result into response object
                response.Data.NoteData = dbResult.Table;

                //encrypt the noteId
                response.Data.NoteData = response.Data.NoteData.Select(e =>
                {
                    e.NoteId = _encryption.AesEncrypt(e.NoteId);
                    return e;
                }).ToList();

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo($"Exception occur during AutopopulatedNoteCommandHandler ---------------- {ex.Message}{Environment.NewLine} {ex.StackTrace} ", _loginUserId);

                return new CommonResponse<AutopopulatedNotes>();

            }
        }
    }
}
