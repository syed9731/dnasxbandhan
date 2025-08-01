namespace DNAS.Domian.DAO.DbHelperModels.DraftNotes
{
    public class ProcFetchDraftListInput
    {
        public string @StartDate { get; set; } = string.Empty;
        public string @EndDate { get; set; } = string.Empty;
        public string @Category { get; set; } = string.Empty;
        public int @UserId { get; set; } = 0;
    }
}
