namespace DNAS.Domian.DTO.UserMaster
{
    public class FyiUserModel
    {
        public UsersModel fyiUsersModel { get; set; } = new();
    }
    public class UsersModel
    {
        public string UserId { get; set; } = string.Empty;
        public string ManagerId { get; set; } = string.Empty;
        public string UserEmpId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DesignationId { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string LastLoginTime { get; set; } = string.Empty;
        public string LastPassRecoveryTime { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
    }
}
