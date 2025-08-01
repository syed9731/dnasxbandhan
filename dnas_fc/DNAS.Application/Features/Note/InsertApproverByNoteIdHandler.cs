using DNAS.Application.Common.Interface;
using DNAS.Application.IDapperRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DAO.DbHelperModels.Approver;
using DNAS.Domian.DTO.Approver;
using DNAS.Domian.DTO.Note;
using MediatR;


namespace DNAS.Application.Features.Note
{
    public class InsertApproverByNoteIdCommand(NoteModel note) : IRequest<NoteModel>
    {
        public NoteModel _note { get; set; } = note;
    }
    internal sealed class InsertApproverByNoteIdHandler(ICustomLogger logger, IEncryption iEncryption, IDapperFactory iDapperFactory) : IRequestHandler<InsertApproverByNoteIdCommand, NoteModel>
    {
        public readonly ICustomLogger _logger = logger;
        private readonly IEncryption _iEncryption = iEncryption;
        private readonly IDapperFactory _iDapperFactory = iDapperFactory;
        public async Task<NoteModel> Handle(InsertApproverByNoteIdCommand request, CancellationToken cancellationToken)
        {

            NoteModel Response = new();
            try
            {
                NoteModel note = new NoteModel();
                note.ApproverIdList = request._note.ApproverIdList;
                note.UserId = request._note.UserId;
                note.NoteId = _iEncryption.AesDecrypt(request._note.NoteId);

                ProcFetchApproverByNoteIdInput InParams = new()
                {
                    @NoteId = note.NoteId,
                    @Approval = note.ApproverIdList
                };
                if (!string.IsNullOrEmpty(request._note.ApproverIdList))
                {
                    string[] values = note.ApproverIdList is not null ? note.ApproverIdList.Split(',') : [];
                    ApproverForDraftModel DbResult = await _iDapperFactory.ExecuteSpDapperAsync<ApproverForDraft, ApproverForDraftModel>(
                        SpName: OraStoredProcedureNames.ProcFetchApproverByNoteId, Params: InParams);
                    if (DbResult != null && values.Length == DbResult.approverForDraft.Count())
                    {
                        _logger.LogwriteInfo("Update Note command successfully done", $"User_{request._note.UserId}");
                        return Response;
                    }
                    else
                    {
                        _logger.LogwriteInfo("Update Note command failed", $"User_{request._note.UserId}");
                        return Response = new();
                    }
                }
                else
                {
                    _logger.LogwriteInfo("ApproverList is empty.", $"User_{request._note.UserId}");
                    return Response = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteError("InsertApproverByNoteIdCommand command exception: "
                    +Environment.NewLine+ ex.Message +Environment.NewLine+ ex.StackTrace, $"User_{request._note.UserId}");
                return Response;
            }
        }
    }
}
