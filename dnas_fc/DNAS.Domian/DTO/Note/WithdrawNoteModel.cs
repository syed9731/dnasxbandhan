using DNAS.Domain.DTO.Note.Common;
using DNAS.Domian.DTO.Note;
using Microsoft.AspNetCore.Http;

namespace DNAS.Domain.DTO.Note
{
    public class WithdrawNoteModel
    {
        public WithNotesModel noteModel { get; set; } = new WithNotesModel();
        public IEnumerable<WithApproversModel> approverModel { get; set; } = new List<WithApproversModel>();
        public IEnumerable<WithNoteComment> commentModel { get; set; } = new List<WithNoteComment>();
        public IEnumerable<WithAttachment> attachmentsModel { get; set; } = new List<WithAttachment>();
        public IEnumerable<WithRecomendedApproverModel> recomendedapproverModel { get; set; } = new List<WithRecomendedApproverModel>();

        public NoteComment querymodel { get; set; } = new NoteComment();
        public CreatorDelegate creatorDelegate { get; set; } = new CreatorDelegate();
        public SkippApprover skippApprover { get; set; }=new SkippApprover();
        public bool IsNoteApprovedDataExist { get; set; }
    }
    public class WithNotesModel
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
        public List<IFormFile>? AttachFiles { get; set; }            
    }
    public class WithApproversModel:CommonApproverModel
    {
    }
    public class WithNoteComment
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
    public class WithAttachment
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string AttachmentPath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
    }
	public class CreatorDelegate
	{
		public string creatorid { get; set; } = string.Empty;
		public string oldapproverid { get; set; } = string.Empty;
	}
	public class SkippApprover
	{
		public string NoteId { get; set; } = string.Empty;
		public string UserId { get; set; } = string.Empty;
	}

	public class WithRecomendedApproverModel:CommonRecomendedApproverModel
    {
        
    }
}
