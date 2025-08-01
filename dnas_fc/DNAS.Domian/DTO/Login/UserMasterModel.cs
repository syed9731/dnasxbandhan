using System.ComponentModel.DataAnnotations;


namespace DNAS.Domian.DTO.Login
{
    public class UserMasterModel
    {       
        public int UserId { get; set; }
        [Required(ErrorMessage = "ManagerId is required.")]
        public int ManagerId { get; set; }
        [Required(ErrorMessage = "Employee-ID is required.")]
        [StringLength(20, ErrorMessage = "Maximum 20 characters allowed")]
        public string UserEmpId { get; set; } = string.Empty;
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string FirstName { get; set; } = string.Empty;
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string MiddleName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters allowed")]
        public string UserName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[A-Za-z0-9._%+-]*@[A-Za-z0-9.-]*\\.[A-Za-z0-9-]{2,}$",ErrorMessage = "Email is required and must be properly formatted.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters allowed")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Designation is required.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string Designation { get; set; } = string.Empty;
        [Required(ErrorMessage = "Grade is required.")]
        [StringLength(20, ErrorMessage = "Maximum 20 characters allowed")]
        public string Grade { get; set; } = string.Empty;
        [Required(ErrorMessage = "Department is required.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters allowed")]
        public string DepartmentId { get; set; } = string.Empty;
        public DateTime LastLoginTime { get; set; } = DateTime.Now;
        public DateTime LastPassRecoveryTime { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public string NetureOfExpenseId { get; set; }= string.Empty;
        public decimal TotalAmount {  get; set; } = decimal.Zero;
        public string Department { get; set; } = string.Empty;
        public string ApproverId { get; set; } = string.Empty;
        public string DesignationId { get; set; } = string.Empty;
        public string ExpenseIncurredAtId {  get; set; } = string.Empty;
        public string NameAseUsr { get; set; } = string.Empty;
        public string PasAseUsr { get; set; } = string.Empty;
        public string EnSheKeValue { get; set; } = string.Empty;
        public int TimeDeference {  get; set; } 
        public string SessionId { get; set; } = string.Empty;
        public string LoginType {  get; set; } = string.Empty;

        [Required(ErrorMessage = "CaptchaCode is required.")]    
        public string captchaCode { get; set; } = string.Empty;

        public string ApproverType {  get; set; } = string.Empty;
    }
}
