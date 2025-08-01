namespace DNAS.Domian.DAO.DbHelperModels.MailNotification
{    
    public class ProcFetchNoteApproversAndCreatorInparam
    {
        public string @NoteId { get; set; } = string.Empty;
    }
    public class ProcFetchDataForFyiMailSend
    {
        public string @NoteId { get; set; } = string.Empty;
        public string @WhoTagged { get; set; } = string.Empty;
        public string @ToWhome { get; set; } = string.Empty;
    }
    public class ProcFetchCreatorApproverNoteInparam
    {
        public string @NoteId { get; set; } = string.Empty;
        public string  @ApproverUserId { get; set; } = string.Empty;
    }
    public class ProcFetchForDelegateMailInparam
    {
        public string @NoteId { get; set; } = string.Empty;
        public string @WhoDelegate { get; set; } = string.Empty;
        public string @ToWhome { get; set; } = string.Empty;
    }
    public class ProcFetchApproverForMailSendInparam
    {
        public string @NoteId { get; set; } = string.Empty;
    }
}
