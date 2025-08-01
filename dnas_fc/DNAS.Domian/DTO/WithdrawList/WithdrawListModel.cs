
using System.Globalization;

namespace DNAS.Domian.DTO.WithdrawList
{
    public class WithdrawListModel
    {
        public IEnumerable<WithdrawListOutModel> withdrawListOutModels { get; set; } = [];
        public FilterWithdrawListNote withdrawlist { get; set; } = new FilterWithdrawListNote();        
    }
    public class WithdrawListOutModel
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string ExpenseIncurredAtId { get; set; } = string.Empty;
        public string NatureOfExpensesId { get; set; } = string.Empty;
        public string CreatorDepartmentId { get; set; } = string.Empty;
        public string NoteState { get; set; } = string.Empty;
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
    public class FilterWithdrawListNote
    {
        public int UserId { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Category { get; set; } = string.Empty;
    }
}
