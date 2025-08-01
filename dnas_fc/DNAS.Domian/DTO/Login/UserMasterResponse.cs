namespace DNAS.Domian.DTO.Login
{
    public class UserMasterResponse
    {
        public string UserName { get; set; }= string.Empty;
        public long UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }=string.Empty;
        public DateTime LastloginTime {  get; set; }
        public string SessionId { get; set; }=string.Empty ;
    }
}
