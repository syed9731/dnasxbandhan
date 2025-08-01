namespace DNAS.Domain.DTO.Approver
{
    public class FetchApproverModel
    {
        public IEnumerable<FetchApproverForCheckApprover> fetchApproverForCheckApprover { get; set; } = [];
    }
    public class NoteIdForFetchApproverModel
    {
        public string NoteId { get; set; } = string.Empty;
    }
    public class FetchApproverForCheckApprover
    {
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int NoOfApprover { get; set; } = 0;
    }
}
