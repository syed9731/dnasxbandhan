using DNAS.Domain.DTO.Amendment;
using DNAS.Domain.DTO.Note;
using DNAS.Domian.DTO.Category;
using DNAS.Domian.DTO.Login;
using DNAS.Domian.DTO.Note;

using MediatR;

namespace DNAS.Application.IRepository
{
    public interface INote
    {
        ValueTask<IEnumerable<CategoryRespModel>> FetchCategory();
        ValueTask<IEnumerable<ExpenseIncurredAtModel>> FetchExpenseIncurredAt(object inparam);
        ValueTask<IEnumerable<NatureOfExpensesModel>> FetchNatureOfExpenses(object inparam);
        ValueTask<IEnumerable<UserMasterModel>> FetchNonFinancialApprover(object inparam);
        ValueTask<IEnumerable<UserMasterModel>> FetchFinancialApprover(object inparam);
        ValueTask<DraftNoteModel> FetchSaveNoteData(object inparam);
        ValueTask<int> DeleteDraftNote(int Noteid);
        ValueTask<PendingNoteModel> FetchPendingNote(object inparam);
        ValueTask<WithdrawNoteDetailsModel> FetchWithdrawDetailsNote(object inparam);
        ValueTask<ViewsNoteModel> FetchViewsNote(object inparam);
        ValueTask<WithdrawNoteModel> FetchWithdrawNote(object inparam);

        //ValueTask<MyApprovedNoteModel> FetchMyApprovedNote(object inparam);
        ValueTask<MyApprovedNoteModel> FetchMyApprovedNote(string NoteId, string UserId);

        //ValueTask<RequestApproverNoteModel> FetchApprovalRequestNote(object inparam); //request._note.NoteId, request._note.UserId
        ValueTask<RequestApproverNoteModel> FetchApprovalRequestNote(string NoteId, string UserId);
        ValueTask<DelegateNoteModel> FetchDelegateNoteDetails(object inparam);
        ValueTask<ViewFyiNoteModel> FetchFyiNoteData(object inparam);
        ValueTask<PendingNoteModel> FetchNoteStatus(object inparam);
        ValueTask<IEnumerable<UserMasterModel>> FetchRecomendedApproverList(object inparam);
        ValueTask<NoteAmendmentModel> FetchAmendmentData(object inparam);
        ValueTask<IEnumerable<UserMasterModel>> FetchReviewerApproverList(object inparam);
        ValueTask<MyApprovedNoteModel> FetchApprovedNote(object inparam);
    }
}