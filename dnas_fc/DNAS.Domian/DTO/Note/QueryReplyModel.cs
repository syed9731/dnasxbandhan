namespace DNAS.Domian.DTO.Note
{
    public class QueryReplyModel
    {
        public ApproverForQuery approverForQuery {  get; set; }= new ApproverForQuery();
        public UserForQuery userForQuery { get; set; } = new UserForQuery();
    }
    public class ApproverForQuery
    {
        public string ApproverId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class UserForQuery
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
