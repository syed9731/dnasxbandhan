namespace DNAS.Domian.DTO.Login
{
    public class RecoverPasswordResponse
    {        
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserEmpId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string TimeDifference { get; set; } = string.Empty;
    }
}
