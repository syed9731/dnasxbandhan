namespace DNAS.Domian.DAO.DbHelperModels.SearchNotes
{
    public class ProcGetSearchNoteInput
    {
        public int @UserId { get; set; } = 0;
        public string @StartDate { get; set; } = string.Empty;
        public string @EndDate { get; set; } = string.Empty;
        public string @Category { get; set; } = string.Empty;
        public string @Title {  get; set; } = string.Empty;
    }
}