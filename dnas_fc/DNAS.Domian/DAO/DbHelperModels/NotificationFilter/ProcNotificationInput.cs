namespace DNAS.Domian.DAO.DbHelperModels.NotificationFilter
{
    public class ProcNotificationInput
    {
        public string @StartDate { get; set; } = string.Empty;
        public string @EndDate { get; set; } = string.Empty;
        public string @Category { get; set; } = string.Empty;
        public string @NoteStatus { get; set; } = string.Empty;
        public int @UserId { get; set; } = 0;
    }

}
