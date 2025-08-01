namespace ReminderNotification.Model
{
    public class NotificationModel
    {
        public IEnumerable<NotificationUserDetail> NotificationUserDetails { get; set; } = [];
        public IEnumerable<EmailConfig> EmailConfigs { get; set; } = [];
        public MailBodyContent MailBodyContents { get; set; } = new();
    }
    public class NotificationUserDetail
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string CreatorEmail { get; set; } = string.Empty;
        public string ApproverName { get; set; } = string.Empty;
        public string ApproverEmail { get; set; } = string.Empty;
        public string Aging {  get; set; } = string.Empty;
        public string ApproverUserId {  get; set; } = string.Empty;
    }
    public class EmailConfig
    {
        public int ConfigurationId { get; set; } = 0;
        public string ConfigurationFor { get; set; } = string.Empty;
        public string ConfigurationKey { get; set; } = string.Empty;
        public string ConfigurationValue { get; set; } = string.Empty;
    }
    public class MailBodyContent
    {
        public string MailSubject { get; set; } = string.Empty;
        public string MailBody { get; set; } = string.Empty;
    }
}
