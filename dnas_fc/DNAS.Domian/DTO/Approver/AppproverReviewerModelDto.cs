namespace DNAS.Domain.DTO.Approver;

public class AppproverReviewerRequestModelDto
{
    public int ApproverReviewerId { get; set; }
    public int PrefixSuffixValue { get; set; }
    public int AddedBy { get; set; }
    public int NoteId { get; set; }
    public int VisibilityMode { get; set; }
}

public class ApproverReviewerResponseModelDto
{
    public bool IsSuccess { get; set; }
    public bool IsCurrentApprover { get; set; }
    public List<int?>? SuffixPrefixList { get; set; } = [];

}