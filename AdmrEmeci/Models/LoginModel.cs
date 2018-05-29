using System.ComponentModel.DataAnnotations;

namespace AdmrEmeci.Models
{
    public class LoginModel
    {
        [Required]
        public string User { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string Error { get; set; }
    }
}