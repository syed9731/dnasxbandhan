namespace DNAS.Domian.DAO.DbHelperModels.ApprovedNotes
{
    public class ProcGetApprovedNoteInput
    {
        public int @UserId { get; set; } = 0;
        public string @StartDate { get; set; } = string.Empty;
        public string @EndDate { get; set; } = string.Empty;
        public string @Category { get; set; } = string.Empty;
    }
}
