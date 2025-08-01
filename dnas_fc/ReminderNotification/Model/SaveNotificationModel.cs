namespace ReminderNotification.Model
{
    public class SaveNotificationModel
    {
        public long NoteId { get; set; } = 0;
        public string Heading { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int ReceiverUserId { get; set; } = 0;
        public string Action { get; set; } = string.Empty;
    }
}
