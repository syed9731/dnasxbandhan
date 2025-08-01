using System.ComponentModel.DataAnnotations;

namespace DNAS.Domian.DTO.Login
{
    public class RecoverPasswordModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression("^[A-Za-z0-9._%+-]*@[A-Za-z0-9.-]*\\.[A-Za-z0-9-]{2,}$", ErrorMessage = "Email is required and must be properly formatted.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters allowed")]
        public string Email { get; set; } = string.Empty;
    }
}
