using System.ComponentModel.DataAnnotations;
namespace DNAS.WEB.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username Required")]
        public string Username { get; set; }=string.Empty;

        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }= string.Empty;
    }
}
