namespace DNAS.Domian.DTO.MailSend
{
    public class SendBackDataModel
    {
        public NotesCreator notesCreator { get; set; } = new NotesCreator();
        public NotesApprover notesApprover { get; set; } = new NotesApprover();

    }
    public class NotesApprover
    {        
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
