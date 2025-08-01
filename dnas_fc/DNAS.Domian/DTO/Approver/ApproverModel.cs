namespace DNAS.Domian.DTO.Approver
{
    public class ApproverData
    {
        public ApproverModel approver { get; set; } = new();
    }
    public class ApproverModel
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string IsCurrentApprover { get; set; } = string.Empty;
        public string? ApproverType { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
