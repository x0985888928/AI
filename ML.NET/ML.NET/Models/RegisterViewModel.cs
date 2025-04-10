using System.ComponentModel.DataAnnotations;

namespace ML.NET.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "請輸入使用者名稱")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入合法的電子郵件地址")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "請輸入密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required(ErrorMessage = "請確認密碼")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "兩次密碼不一致")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
