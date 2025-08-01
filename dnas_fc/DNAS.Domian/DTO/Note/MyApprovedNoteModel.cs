using DNAS.Domain.DTO.Note.Common;
using DNAS.Domian.DTO.Note;

namespace DNAS.Domain.DTO.Note
{
    public class MyApprovedNoteModel
    {
        public MyAppNotesModel noteModel { get; set; } = new MyAppNotesModel();
        public IEnumerable<MyAppApproversModel> approverModel { get; set; } = new List<MyAppApproversModel>();
        public IEnumerable<MyAppNoteComment> commentModel { get; set; } = new List<MyAppNoteComment>();
        public IEnumerable<MyAppAttachment> attachmentsModel { get; set; } = new List<MyAppAttachment>();
        public IEnumerable<MyAppRecomendedApproverModel> recomendedapproverModel { get; set; } = new List<MyAppRecomendedApproverModel>();
        public NoteModel onlyNoteModel { get; set; } = new NoteModel();

        public NoteComment querymodel { get; set; } = new NoteComment();
        public CreatorDelegate creatorDelegate { get; set; } = new CreatorDelegate();
        public SkippApprover skippApprover { get; set; }=new SkippApprover();
        public bool IsNoteApprovedDataExist { get; set; }

    }
    public class MyAppNotesModel: NoteModel
    {
        public string CreatorDepartmentId { get; set; } = string.Empty;        
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
    public class MyAppApproversModel : CommonApproverModel
    {
    }
    public class MyAppNoteComment
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
    public class MyAppAttachment
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string AttachmentPath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
    }
	public class MyAppCreatorDelegate
    {
		public string creatorid { get; set; } = string.Empty;
		public string oldapproverid { get; set; } = string.Empty;
	}
	public class MyAppSkippApprover
    {
		public string NoteId { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
	}

	public class MyAppRecomendedApproverModel : CommonRecomendedApproverModel
    {
        
    }
}
