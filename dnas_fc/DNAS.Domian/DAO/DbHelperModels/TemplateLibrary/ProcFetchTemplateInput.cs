namespace DNAS.Domian.DAO.DbHelperModels.TemplateLibrary
{
    public class ProcFetchTemplateInput
    {
        public int @UserId { get; set; } = 0;
        public string @StartDate { get; set; } = string.Empty;
        public string @EndDate { get; set; } = string.Empty;
        public string @Category { get; set; } = string.Empty;
    }
}
