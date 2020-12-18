using System.ComponentModel.DataAnnotations;

namespace WebApi.BLL.DTO
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Email is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; } = false;
    }
}
