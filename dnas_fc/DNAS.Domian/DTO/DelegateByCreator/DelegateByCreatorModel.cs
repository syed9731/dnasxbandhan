namespace DNAS.Domain.DTO.DelegateByCreator
{
    public class DelegateByCreatorModel
    {
        public NewApprover newApprover {  get; set; }= new NewApprover();
        public OldApprover oldApprover { get; set; } = new OldApprover();
        public IEnumerable<ApproverList> approverlist { get; set; } = [];
        public CreatorDetails creatorDetails { get; set; }=new CreatorDetails();
        public NoteDetails noteDetails { get; set; }=new NoteDetails();
    }
    public class NewApprover
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class OldApprover
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
    }
    public class ApproverList
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string IsCurrentApprover { get; set; } = string.Empty;
        public string AssignTime { get; set; } = string.Empty;
        public string ApproverType { get; set; } = string.Empty;
    }
    public class CreatorDetails
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmpId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
    }
    public class NoteDetails
    {
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string ExpenseIncurredAtId { get; set; } = string.Empty;
        public string NatureOfExpensesId { get; set; } = string.Empty;
        public string NoteState { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CapitalExpenditure { get; set; } = string.Empty;
        public string OperationalExpenditure { get; set; } = string.Empty;
        public string TotalAmount { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
    }
}
