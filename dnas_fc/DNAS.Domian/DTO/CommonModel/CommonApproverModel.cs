namespace DNAS.Domain.DTO.CommonModel
{
    public class CommonApproverModel
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string IsCurrentApprover { get; set; } = string.Empty;
        public string AssignTime { get; set; } = string.Empty;
        public string ApproverType { get; set; } = string.Empty;
        public string SkippBy { get; set; } = string.Empty;
        public string SkippTime { get; set; } = string.Empty;
    }
}
