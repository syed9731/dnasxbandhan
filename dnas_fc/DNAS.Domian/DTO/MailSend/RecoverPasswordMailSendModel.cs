namespace DNAS.Domian.DTO.MailSend
{
    public class RecoverPasswordMailSendModel
    {
        public string Sender { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserEmpId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
