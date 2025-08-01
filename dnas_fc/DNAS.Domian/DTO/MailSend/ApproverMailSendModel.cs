namespace DNAS.Domian.DTO.MailSend
{
    public class ApproverMailSendModel
    {
        public NotesCreator notesCreator {  get; set; }= new NotesCreator();
        public NotesApprover1 notesApprover { get; set; } = new NotesApprover1();
    }
    public class NotesApprover1
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
