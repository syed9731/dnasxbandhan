

using ReminderNotification.Model;

namespace ReminderNotification.DAL
{
    internal interface IFetchUsers
    {
        Task<NotificationModel> GetList();
    }
}
