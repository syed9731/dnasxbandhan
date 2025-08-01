namespace DNAS.Domian.DAO.DbHelperModels.DeletegateApprover
{
    public class ProcFetchUserAndApproverForDelegateInput
    {
        public string @SearchKey { get; set; } = string.Empty;
        public string @NoteId { get; set; } = string.Empty;
        public string @UserId { get; set; } = string.Empty;
    }
    public class ProcFetchApproverListForDelegateInput
    {
        public string @NoteId { get; set; } = string.Empty;
    }
}
