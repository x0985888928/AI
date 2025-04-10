using System.ComponentModel.DataAnnotations;

namespace ML.NET.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "請輸入使用者帳號")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "記住我")]
        public bool RememberMe { get; set; }
    }
}
