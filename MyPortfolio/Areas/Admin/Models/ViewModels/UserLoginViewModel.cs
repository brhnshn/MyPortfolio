using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Areas.Admin.Models
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adını giriniz")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifrenizi giriniz")]
        public string Password { get; set; }
    }
}