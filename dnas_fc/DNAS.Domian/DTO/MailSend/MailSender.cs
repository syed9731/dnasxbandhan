namespace DNAS.Domian.DTO.MailSend
{
    public class MailSender
    {
        public string Sender { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string CC {  get; set; } = string.Empty;
        public string SMTPHost { get; set; } = string.Empty;
        public int SMTPPort { get; set; } 
        public string? SMTPLoginUserID { get; set; } = string.Empty;
        public string? SMTPPassword { get; set; } = string.Empty;
        public bool SSL { get; set; } = true;
        public string[] ?atchfile { get; set; }
    }
}
