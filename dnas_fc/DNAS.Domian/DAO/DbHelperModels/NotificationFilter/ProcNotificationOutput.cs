using DNAS.Domian.DTO.Draft;

namespace DNAS.Domian.DAO.DbHelperModels.NotificationFilter
{
    public class ProcNotificationOutput
    {
        public IEnumerable<Notification> NotificationList { get; set; } = [];
    }
}
