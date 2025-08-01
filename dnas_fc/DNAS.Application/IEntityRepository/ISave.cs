using DNAS.Domain.DTO.Amendment;
using DNAS.Domain.DTO.Approver;
using DNAS.Domain.DTO.Comment;
using DNAS.Domain.DTO.DelegateByCreator;
using DNAS.Domain.DTO.Login;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.CommonResponse;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.Note;
using DNAS.Domian.DTO.Notification;
using DNAS.Domian.DTO.Template;

namespace DNAS.Application.IRepository
{
    public interface ISave
    {
        Task<CommonResponse<CommonResp>> SavedData(Domian.DTO.FYI.FyiModel Request);
        Task<NoteModel> SaveNote(NoteModel Request);
        Task<string> SaveTemplate(TemplateModel Request);
        ValueTask<bool> SaveQueryInitiate(PendingNoteModel model);
        ValueTask<bool> SaveSendBack(PendingNoteModel Request);
        ValueTask<bool> SaveForYourInformatinData(PendingNoteModel model);
        ValueTask<string> SaveDelegateAndUpdateApprover(DelegateAsignModel model);
        ValueTask<string> SaveNotificationData(NotificationModel model);
        ValueTask<bool> SaveQueryReply(WithdrawNoteModel model);
        ValueTask<NoteModel> InsertNoteTitleData(NoteModel Request);
        ValueTask<NoteModel> InsertNotecategoryData(NoteModel Request);
        ValueTask<NoteModel> InsertExpenseIncuredAtData(NoteModel Request);
        ValueTask<NoteModel> InsertNatureOfExpData(NoteModel Request);
        ValueTask<NoteModel> InsertNoteOpexData(NoteModel Request);
        ValueTask<NoteModel> InsertNoteCapexData(NoteModel Request);
        ValueTask<NoteModel> InsertNoteBodyData(NoteModel Request);
        ValueTask<NoteModel> InsertApproverByNoteIdData(NoteModel Request);
        ValueTask<NoteModel> InsertSelectedApproverWthoutNoteIdData(NoteModel Request);
        ValueTask<bool> SaveUserTracking(UserTrackingModel model);
        ValueTask<bool> SaveApproveComment(CommentReqModel model);
        ValueTask<bool> SaveAttachment(WithNotesModel Request);
        ValueTask<bool> SaveAsignDelegateComment(AsignDelegateNoteInputModel Request);
        ValueTask<bool> SaveDelegateByCreator(DelegateByCreatorModel model);
        ValueTask<bool> TransferToNoteApproved(string noteid);
        ValueTask<ApproverReviewerResponseModelDto> SaveReviewerApprover(AppproverReviewerRequestModelDto request);
        ValueTask<AmendmentNoteModel> SaveAmendNote(AmendmentNoteModel request);
    }
}
