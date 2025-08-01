namespace DNAS.Domain.DTO.Login
{
    public class UserTrackingData
    {
        public UserTrackingModel UserTracking { get; set; } = new();
    }

    public class UserTrackingModel
    {
        public int UserId { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime LastLoginTime { get; set; } = DateTime.Now.AddHours(-1);
        public int TimeDifference { get; set; }
        public string SessionId { get; set; } = string.Empty;
    }
}
