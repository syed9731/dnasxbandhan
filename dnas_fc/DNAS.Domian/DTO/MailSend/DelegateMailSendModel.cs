namespace DNAS.Domian.DTO.MailSend
{
    public class DelegateMailSendModel
    {
        public NotesCreator notesCreator { get; set; } = new NotesCreator();
        public DelegateSender delegateSender { get; set; } = new DelegateSender();
        public DelegateReceiver delegateReceiver { get; set; } = new DelegateReceiver();
    }  
    public class DelegateSender
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class DelegateReceiver
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
