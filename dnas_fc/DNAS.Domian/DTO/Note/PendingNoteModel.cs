using DNAS.Domain.NoSpecialCharacter;

namespace DNAS.Domian.DTO.Note
{
    public class PendingNoteModel
    {
        public NotesModel noteModel { get; set; } = new NotesModel();
        public IEnumerable<ApproversModel> approverModel { get; set; } = new List<ApproversModel>();
        public IEnumerable<NoteComment> commentModel { get; set; } = new List<NoteComment>();
        public IEnumerable<Attachment> attachmentsModel { get; set; } = new List<Attachment>();
        public NoteComment querymodel { get; set; } = new NoteComment();
        public FyiModel fyiModel { get; set; } = new FyiModel();
    }
    public class NotesModel
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
    }
    public class ApproversModel
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public DateTime ApprovedTime { get; set; }
        public string IsCurrentApprover { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public int Deligated_UserId { get; set; }
    }
    public class NoteComment
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
    public class Attachment
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string AttachmentPath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
    }
    public class FyiModel
    {
        public string NoteId { get; set; } = string.Empty;
        public string WhoTagged { get; set; } = string.Empty;
        public string ToWhome { get; set; } = string.Empty;
        public string TaggedTime { get; set; } = string.Empty;
    }
}
