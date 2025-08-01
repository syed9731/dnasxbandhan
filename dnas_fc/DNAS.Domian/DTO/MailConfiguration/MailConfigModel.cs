namespace DNAS.Domian.DTO.MailConfiguration
{
    public class MailConfigModel
    {
        public MailConfig mailConfig {  get; set; }=new MailConfig();
    }
    public class MailConfig
    {
        public string MailKey { get; set; } = string.Empty;
        public string MailSubject { get; set; } = string.Empty;
        public string MailBody { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
    }
}
