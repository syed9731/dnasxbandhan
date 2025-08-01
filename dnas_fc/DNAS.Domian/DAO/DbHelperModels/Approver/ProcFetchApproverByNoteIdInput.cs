namespace DNAS.Domian.DAO.DbHelperModels.Approver
{
    public class ProcFetchApproverByNoteIdInput
    {
        public string @NoteId {  get; set; }=string.Empty;
        public string @Approval {  get; set; }=string.Empty;
        public string @RecomendedApproval {  get; set; }=string.Empty;
    }
    public class PorcFetchRecomendedApproverByNoteIdInput
    {
        public string @NoteId { get; set; } = string.Empty;
        public string @Approval { get; set; } = string.Empty;
    }
}
