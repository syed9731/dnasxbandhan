namespace DNAS.Domian.DTO.MailSend
{
    public class FyiDataModel
    {
        public NotesCreator notesCreator {  get; set; }=new NotesCreator();
        public FyiSender fyiSender { get; set; } =new FyiSender();
        public FyiReceiver fyiReceiver { get; set; }=new FyiReceiver(); 
    }
    public class NotesCreator
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NoteTitle {  get; set; } = string.Empty;
    }
    public class FyiSender
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class FyiReceiver
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
