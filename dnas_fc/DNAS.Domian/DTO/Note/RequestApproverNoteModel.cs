using DNAS.Domain.DTO.Note.Common;
using DNAS.Domain.NoSpecialCharacter;

namespace DNAS.Domain.DTO.Note
{
    public class RequestApproverNoteModel
    {
        public ReqNotesModel noteModel { get; set; } = new ReqNotesModel();
        public IEnumerable<ReqApproversModel> approverModel { get; set; } = new List<ReqApproversModel>();
        public IEnumerable<ReqNoteComment> commentModel { get; set; } = new List<ReqNoteComment>();
        public IEnumerable<ReqAttachment> attachmentsModel { get; set; } = new List<ReqAttachment>();
        public IEnumerable<ReqRecomendedApproverModel> recomendedapproverModel { get; set; } = new List<ReqRecomendedApproverModel>();


        public ReqNoteComment querymodel { get; set; } = new ReqNoteComment();
        public bool IsNoteApprovedDataExist { get; set; }
    }
    public class ReqNotesModel
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
        public bool IsCurrentApprover { get; set; } = false;
    }
    public class ReqApproversModel:CommonApproverModel
    {

    }
    public class ReqNoteComment
    {
        public string NoteTackerId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string ApproverId { get; set; } = string.Empty;

        [NoSpecialCharacter(ErrorMessage = "Comment contains special character!")]
        public string Comment { get; set; } = string.Empty;
        public DateTime CommentTime { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
    public class ReqAttachment
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string AttachmentPath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
    }

    public class ReqRecomendedApproverModel: CommonRecomendedApproverModel
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
