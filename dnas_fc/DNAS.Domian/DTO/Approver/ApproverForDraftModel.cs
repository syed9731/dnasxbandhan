namespace DNAS.Domian.DTO.Approver
{
    public class ApproverForDraftModel
    {
        public IEnumerable<ApproverForDraft> approverForDraft { get; set; } = [];
    }
    public class ApproverForDraft
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string IsCurrentApprover { get; set; } = string.Empty;
    }
}
