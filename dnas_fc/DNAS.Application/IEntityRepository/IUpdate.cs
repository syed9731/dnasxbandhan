using DNAS.Domain.DTO.Approver;
using DNAS.Domain.DTO.Login;
using DNAS.Domain.DTO.Note;
using DNAS.Domain.DTO.SendBack;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.CommonResponse;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Template;

namespace DNAS.Application.IRepository
{
    public interface IUpdate
    {
        Task<CommonResponse<CommonResp>> UpdatePassword(UserMasterModel Request);
        Task<bool> UpdateNoteTitleData(NoteModel Request);
        Task<bool> UpdateNoteBodyData(NoteModel Request);
        Task<bool> UpdateNoteCategoryData(NoteModel Request);
        Task<bool> UpdateNoteExpenseIncurredAtData(NoteModel Request);
        Task<bool> UpdateNoteNatureOfExpensesData(NoteModel Request);
        Task<bool> UpdateNoteCapexData(NoteModel Request);
        Task<bool> UpdateNoteOpexData(NoteModel Request);
        Task<bool> UpdateTemplateData(TemplateModel Request);
        Task<string> DeleteTemplateData(TemplateModel Request);
        ValueTask<bool> UpdateNoteStatusData(PendingNoteModel Request);
        ValueTask<bool> UpdateApproverData(Domian.DTO.Approver.ApproverModel Request);
        ValueTask<bool> UpdateNoteData(NoteModel Request);
        ValueTask<bool> UpdateUserTracking(UserTrackingModel Request);
        ValueTask<bool> UpdateLatestLoginTime(int UserId);
        ValueTask<bool> UpdateForNextApprover(SkippByCreatorModel Request);
        ValueTask<bool> UpdateRollBackNextApprover(SkippByCreatorModel Request);
        ValueTask<bool> UpdateCurrentApproverAsSkippedApprover(SkippByCreatorModel Request);
        ValueTask<ApproverDtlModel> UpdateSendBackNote(SendBackNoteDto Request);
        ValueTask<bool> RevartBackNoteStatus(string noteId, string approverId);
        ValueTask<bool> ResetPreviousNoteDetails(string noteId);
    }
}
