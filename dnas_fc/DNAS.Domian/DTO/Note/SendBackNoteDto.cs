using DNAS.Domain.DTO.Attachment;
using DNAS.Domian.DTO.Note;

namespace DNAS.Domain.DTO.Note
{
    public class SendBackNoteDto
    {
        public NoteModel? NoteModel { get; set; } = new();
        public string ExpenseIncurredAtName {  get; set; }=string.Empty;
        public string NatureOfExpensesName { get; set; } = string.Empty;
        public string CapitalExpenditure { get; set; } = string.Empty;
        public string OperationalExpenditure { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public IList<SendBackNoteApproverModel>? ApproverList { get; set; } = [];
        public IList<AttachmentDto>? AttachmentList { get; set; } = [];

        // storing ApproverList as JSON
        public string ApproverListJson { get; set; } = string.Empty;

        // storing AttachmentList as JSON
        public string AttachmentListJson { get; set; } = string.Empty;
        /* 
         Singleton instance of an empty NoteModel
         Reuse the singleton instance (Subhrajit 13-09-2024)
        */
        public static readonly SendBackNoteDto Empty = new();
    }

    public class SendBackNoteApproverModel
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ApproverType { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role {  get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;

    }

}
