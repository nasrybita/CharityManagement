using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Ui.ViewModels
{
    public class LoginRequestViewModel
    {
        [Required(ErrorMessage = "نام کاربری الزامی است")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "نام کاربری باید بین 3 تا 100 کاراکتر باشد")]
        public string UserName { get; set; } = string.Empty;


        [Required(ErrorMessage = "رمز عبور الزامی است")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد")]
        public string Password { get; set; } = string.Empty;
    }
}
