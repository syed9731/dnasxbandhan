namespace DNAS.Domain.DTO.Note
{
    public class FetchNoteForAttachmentModel
    {
        public FetchNote fetchNote { get; set; } = new();
        public AttachmentCount attachmentCount { get; set; } = new();
    }
    public class FetchNote
    {
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string ExpenseIncurredAtId { get; set; } = string.Empty;
        public string NatureOfExpensesId { get; set; } = string.Empty;
        public string CreatorDepartment { get; set; } = string.Empty;
        public string NoteState { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CapitalExpenditure { get; set; } = string.Empty;
        public string OperationalExpenditure { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string NoteBody { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string WithdrawDate { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
        public string NoteUID { get; set; } = string.Empty;
    }
    public class AttachmentCount
    {
        public int AttachCount { get; set; }
    }
}
