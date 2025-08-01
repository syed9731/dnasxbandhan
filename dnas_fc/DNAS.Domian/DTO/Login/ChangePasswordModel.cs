using System.ComponentModel.DataAnnotations;

namespace DNAS.Domian.DTO.Login
{
    public class ChangePasswordModel
    {
        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Predefined Password is required.")]
        public string predefinedpassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&#])[A-Za-z\\d@$!%*?&#]{8,15}$", ErrorMessage = "Minimum 8 and maximum 15 characters, at least one uppercase letter, one lowercase letter, one number and one special character.")]
        [StringLength(15, ErrorMessage = "Password cannot exceed 15 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}