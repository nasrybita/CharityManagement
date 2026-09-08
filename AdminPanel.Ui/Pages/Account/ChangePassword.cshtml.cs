using System.ComponentModel.DataAnnotations;

using AdminPanel.Application.Interfaces;
using AdminPanel.Application.Security;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Account
{
    public class ChangePasswordModel : PageModel
    {


        private readonly IHttpClientFactory _httpClientFactory;

        public ChangePasswordModel(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        //public bool PasswordChanged { get; set; }
        [TempData]
        public bool PasswordChanged { get; set; }


        public bool RequireCurrentPassword
        {
            get
            {
                return User.Identity?.IsAuthenticated == true;
            }
        }


        [BindProperty]
        public InputModel Input { get; set; } = new();


        public class InputModel
        {
            [Display(Name = "رمز عبور فعلی")]
            [DataType(DataType.Password)]
            public string? CurrentPassword { get; set; }


            [Required(ErrorMessage = "وارد کردن رمز عبور جدید الزامی است.")]
            [StringLength(
                100,
                MinimumLength = 6,
                ErrorMessage = "رمز عبور باید حداقل ۶ کاراکتر باشد."
            )]
            [DataType(DataType.Password)]
            public string NewPassword { get; set; } = null!;


            [Required(ErrorMessage = "تکرار رمز عبور الزامی است.")]
            [Compare(
                "NewPassword",
                ErrorMessage = "رمز جدید و تکرار آن مطابقت ندارند."
            )]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; } = null!;
        }



        public void OnGet()
        {

        }



        


            public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var userIdClaim =
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;


            if (!int.TryParse(userIdClaim, out int userId))
            {
                ModelState.AddModelError(
                    "",
                    "کاربر جاری شناسایی نشد."
                );

                return Page();
            }


            var client = _httpClientFactory.CreateClient("ApiClient");


            var response = await client.PutAsJsonAsync(
                $"api/User/reset-password/{userId}",
                new
                {
                    CurrentPassword = Input.CurrentPassword,
                    NewPassword = Input.NewPassword,
                    ConfirmPassword = Input.ConfirmPassword
                });


            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    "",
                    "تغییر رمز عبور انجام نشد."
                );

                return Page();
            }


            PasswordChanged = true;


            Console.WriteLine("PASSWORD CHANGED SUCCESS");


            return Page();
        }




    }
    }
