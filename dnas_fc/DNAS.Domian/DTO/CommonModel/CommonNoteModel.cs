namespace DNAS.Domain.DTO.CommonModel
{
    public class CommonNoteModel
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
    }
}
