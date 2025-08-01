namespace DNAS.Domain.DTO.Comment
{
    public class CommentReqModel
    {
        public string NoteId { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string ApproverId { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string CommentTime { get; set; } = string.Empty;
    }
}
