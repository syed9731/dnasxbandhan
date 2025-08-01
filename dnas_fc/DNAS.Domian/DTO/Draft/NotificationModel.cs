using System.Globalization;

namespace DNAS.Domian.DTO.Draft
{
    public class NotificationData
    {
        public IEnumerable<Notification> NotificationList { get; set; } = [];
        public FilterNotification FilterNotifications { get; set; } = new();
    }

    public class Notification
    {
        public string CategoryName { get; set; } = string.Empty;
        public int NotificationId { get; set; } = 0;
        public string Heading { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string NotificationTime { get; set; } = string.Empty;
        public string NoteStatus { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
    }

    public class FilterNotification
    {
        public int Id { get; set; } = 0;
        public string StartDate { get; set; } = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string EndDate { get; set; } = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class HederNotificationsList
    {
        public int NotificationCount { get; set; } = 0;
    }

    public class HeaderNotifications
    {
        public int NotificationId { get; set; } = 0;
        public string Message { get; set; } = string.Empty;
        public string Heading { get; set; } = string.Empty;
        public DateTime NotificationDate { get; set; } = DateTime.MinValue;
    }
}
