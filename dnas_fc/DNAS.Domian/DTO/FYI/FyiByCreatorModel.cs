using DNAS.Domian.DTO.Approver;

namespace DNAS.Domain.DTO.FYI
{
    public class FyiByCreatorModel
    {
        public FyiUsersModel fyiUsersModel { get; set; } = new();
        public IEnumerable<ApproverListForFyi> approverListForFyi { get; set; } = [];
        public IEnumerable<FyiList> fyiList { get; set; } = [];
    }
    public class FyiUsersModel
    {
        public string UserId { get; set; } = string.Empty;
        public string ManagerId { get; set; } = string.Empty;
        public string UserEmpId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DesignationId { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string LastLoginTime { get; set; } = string.Empty;
        public string LastPassRecoveryTime { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
    }
    public class ApproverListForFyi
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
    public class FyiList
    {
        public string FYIId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string WhoTagged { get; set; } = string.Empty;
        public string ToWhome { get; set; } = string.Empty;
        public string TaggedTime { get; set; } = string.Empty;
    }
}
