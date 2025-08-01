using DNAS.Domain.DTO.Login;
using DNAS.Domian.DTO.Approver;
using DNAS.Domian.DTO.Note;

namespace DNAS.Application.IRepository
{
    public interface IDelete
    {
        public Task<string> DeleteApproverData(DNAS.Domian.DTO.Note.ApproverModel Request);
        public ValueTask<string> DeleteApproverByNoteId(ApproverForDraftModel Request);
        public ValueTask<bool> DeleteUserTracking(object inparam);
        public ValueTask<bool> DeleteDuplicateApprover(object inparam);
        public ValueTask<bool> DeleteAttachmentByNoteId(string noteid);
        public ValueTask<int> DeleteAttachment(int NoteId, int AttachmentId);
    }
}
