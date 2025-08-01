namespace DNAS.Domian.DTO.MailSend
{
    public class NoteApprovedModel
    {
        public NoteCreator noteCreator {  get; set; }=new NoteCreator();
        public IEnumerable<NoteApprover> noteApprovers { get; set; }=[];
    }
    public class NoteCreator
    {
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string DateOfCreation { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string NoteUID { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class NoteApprover
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string IsApproved { get; set; } = string.Empty;
        public string ApprovedTime { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
