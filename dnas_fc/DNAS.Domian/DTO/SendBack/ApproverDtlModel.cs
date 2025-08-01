namespace DNAS.Domain.DTO.SendBack
{
    public class ApproverDtlModel
    {
        public ApproverDtl approverDtl { get; set; } = new();
        public CreatorDtl creatorDtl { get; set; } = new();
        public NoteDtl noteDtl { get; set; } = new();
    }
    public class ApproverDtl
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmpId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class CreatorDtl
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmpId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class NoteDtl
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
    }
}
