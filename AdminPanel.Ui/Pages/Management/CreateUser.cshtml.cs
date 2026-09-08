using AdminPanel.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class CreateUserModel : PageModel
    {

        [BindProperty]
        public string CurrentUserTypeClaim { get; set; } = string.Empty;

        [BindProperty]
        public string? CurrentUserCharityIdClaim { get; set; }



        public IActionResult OnGet()
        {

            // Extract the logged-in user's role from the claims
            CurrentUserTypeClaim = User.FindFirst("UserType")?.Value ??
                                      User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? string.Empty;



            // Extract the logged-in user's role charity ID from the claims
            CurrentUserCharityIdClaim = User.FindFirst("CharityId")?.Value ??
                                User.FindFirst("charityId")?.Value ??
                                User.FindFirst("CharityIdClaim")?.Value;



            //A small log for myself in visual studio 
            System.Diagnostics.Debug.WriteLine($"User Type: {CurrentUserTypeClaim}, Charity ID: {CurrentUserCharityIdClaim}");



            // Server-side access check for the frontend:
            // only Root (SystemAdmin) and CharityAdmin can view this page.
            if (CurrentUserTypeClaim != ((int)AdminUserType.AdminSystem).ToString() &&
        CurrentUserTypeClaim != ((int)AdminUserType.CharityAdmin).ToString())
            {
                return RedirectToPage("/Account/AccessDenied");
            }


            return Page();

        }






    }
}
