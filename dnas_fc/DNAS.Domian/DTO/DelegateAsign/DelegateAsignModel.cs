namespace DNAS.Domian.DTO.DelegateAsign
{
    public class DelegateAsignModel
    {
        public FindUserDetails findUserDetails { get; set; } = new FindUserDetails();
        public DelegateApprover delegateApprover { get; set; } =new DelegateApprover();        
        public NoteDetails noteDetails { get; set; }=new NoteDetails();
        public DelegateAsign delegateAsign { get; set; }=new DelegateAsign();
    }
    public class DelegateAsignListModel
    {
        public IEnumerable<DelegateApproverlist> delegateApproverlist { get; set; } = [];
    }
    public class DelegateAsign
    {
        public string DelegateId { get; set; } = string.Empty;
        public string ApproverID { get; set; } = string.Empty;
        public string DeligatedUserId { get; set; } = string.Empty;
        public string AssignTime { get; set; } = string.Empty;
        public string DelegateBy {  get; set; } = string.Empty;
    }
    public class DelegateApprover
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
    }
    public class FindUserDetails
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class NoteDetails
    {
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
    public class DelegateApproverlist
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}