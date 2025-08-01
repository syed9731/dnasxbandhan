namespace DNAS.Domain.DAO.DbHelperModels.Attachment
{
    public class FetchAttachmentModel
    {
        public AttachmentModel attachmentModel { get; set; } = new();
    }
    public class AttachmentModel
    {
        public string AttachmentId { get; set; } = string.Empty;
        public string AttachmentPath { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
    }
}
