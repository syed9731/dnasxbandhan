using DNAS.Domain.DTO.Note.Common;

namespace DNAS.Domain.DTO.Note
{
    public class ViewFyiNoteModel
    {
        public VwFyiNotesModel noteModel { get; set; } = new VwFyiNotesModel();
        public IEnumerable<VwFyiApproversModel> approverModel { get; set; } = new List<VwFyiApproversModel>();
        public IEnumerable<VwFyiNoteComment> commentModel { get; set; } = new List<VwFyiNoteComment>();
        public IEnumerable<VwFyiAttachment> attachmentsModel { get; set; } = new List<VwFyiAttachment>();
        public IEnumerable<VwFyiRecomendedApproverModel> recomendedapproverModel { get; set; } = new List<VwFyiRecomendedApproverModel>();
    }
    public class VwFyiNotesModel
    {
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string ExpenseIncurredAtId { get; set; } = string.Empty;
        public string NatureOfExpensesId { get; set; } = string.Empty;
        public string CreatorDepartmentId { get; set; } = string.Empty;
        public string NoteState { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CapitalExpenditure { get; set; } = string.Empty;
        public string OperationalExpenditure { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string NoteBody { get; set; } = string.Empty;
        public DateTime DateOfCreation { get; set; }
        public DateTime WithdrawDate { get; set; }
        public string NoteStatus { get; set; } = string.Empty;
        public string CurrentApproverId { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
        public string NoteUID { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;

        public string SearchKey { get; set; } = string.Empty;
        public string CreatorUserId { get; set; } = string.Empty;
        public string ExpenseIncurredAtName { get; set; } = string.Empty;
        public string NatureOfExpensesName { get; set; } = string.Empty;
        public string NatureOfExpenseCode { get; set; } = string.Empty;

    }
    public class VwFyiApproversModel:CommonApproverModel
    {
        //public string ApproverId { get; set; } = string.Empty;
        //public string NoteId { get; set; } = string.Empty;
        //public string UserId { get; set; } = string.Empty;
        //public string IsApproved { get; set; } = string.Empty;
        //public DateTime ApprovedTime { get; set; }
        //public string IsCurrentApprover { get; set; } = string.Empty;
        //public string FirstName { get; set; } = string.Empty;
        //public string MiddleName { get; set; } = string.Empty;
        //public string LastName { get; set; } = string.Empty;
        //public string Grade { get; set; } = string.Empty;
        //public string Department { get; set; } = string.Empty;
        //public string DesignationName { get; set; } = string.Empty;
        //public int Deligated_UserId { get; set; }
        //public string SkippBy { get; set; } = string.Empty;
        //public string SkippTime { get; set; } = string.Empty;
    }
    public class VwFyiNoteComment
    {
        public string NoteTackerId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string ApproverId { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime CommentTime { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
    public class VwFyiAttachment
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string AttachmentPath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
    }
    public class VwFyiRecomendedApproverModel:CommonRecomendedApproverModel
    {
        //public string ApproverId { get; set; } = string.Empty;
        //public string NoteId { get; set; } = string.Empty;
        //public string UserId { get; set; } = string.Empty;
        //public string IsApproved { get; set; } = string.Empty;
        //public DateTime ApprovedTime { get; set; }
        //public string IsCurrentApprover { get; set; } = string.Empty;
        //public string FirstName { get; set; } = string.Empty;
        //public string MiddleName { get; set; } = string.Empty;
        //public string LastName { get; set; } = string.Empty;
        //public string Grade { get; set; } = string.Empty;
        //public string Department { get; set; } = string.Empty;
        //public string DesignationName { get; set; } = string.Empty;
        //public int Deligated_UserId { get; set; }
        //public string ApproverType { get; set; } = string.Empty;
        //public string SkippBy { get; set; } = string.Empty;
        //public string SkippTime { get; set; } = string.Empty;
    }
}
