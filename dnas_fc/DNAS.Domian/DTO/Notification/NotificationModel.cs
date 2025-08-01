namespace DNAS.Domian.DTO.Notification
{
    public class NotificationModel
    {
        public string ReceiverUserId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public string Heading { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Action {  get; set; } = string.Empty;
    }
}
