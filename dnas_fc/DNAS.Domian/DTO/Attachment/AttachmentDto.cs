namespace DNAS.Domain.DTO.Attachment
{
    public class AttachmentDto
    {
        public long AttachmentId { get; set; }
        public long NoteId { get; set; }
        public string AttachmentPath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
    }
}
