namespace DNAS.Domain.DTO.Note.Common
{
    public class CommonRecomendedApproverModel
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public DateTime ApprovedTime { get; set; }
        public string IsCurrentApprover { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public int Deligated_UserId { get; set; }
        public string ApproverType { get; set; } = string.Empty;

        public string SkippBy { get; set; } = string.Empty;
        public string SkippTime { get; set; } = string.Empty;
    }
}
