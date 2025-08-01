namespace DNAS.Domian.DAO.DbHelperModels.Approver
{
    public class ProcFetchApproverInput
    {
        public string @NoteId { get; set; } = string.Empty;
        public string @UserId { get; set; } = string.Empty;
    }
    public class ProcFetchTopApproverInput
    {
        public string @NoteId { get; set; } = string.Empty;
    }
    public class ProcFetchApproverAndCreatorInput
    {
        public string @NoteId { get; set; } = string.Empty;
        public string @UserId { get; set; } = string.Empty;
    }
}
