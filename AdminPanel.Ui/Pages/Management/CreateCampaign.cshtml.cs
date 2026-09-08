using AdminPanel.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class CreateCampaignModel : PageModel
    {
        public int UserType { get; private set; }
        public int? CharityId { get; private set; }
        public string? AccessToken { get; private set; }

        public IActionResult OnGet()
        {
            // Read the token and claims from the authenticated user's identity
            AccessToken = User.FindFirst("AccessToken")?.Value;

            var userTypeClaim = User.FindFirst("UserType")?.Value;
            if (int.TryParse(userTypeClaim, out var userTypeVal))
            {
                UserType = userTypeVal;
            }

            var charityIdClaim = User.FindFirst("CharityId")?.Value;
            if (int.TryParse(charityIdClaim, out var charityIdVal))
            {
                CharityId = charityIdVal;
            }

            // Only CharityAdmin and CharityUser are allowed to access the CreateCampaign page
            if (UserType != (int)AdminUserType.CharityAdmin &&
                UserType != (int)AdminUserType.CharityUser)
            {
                return RedirectToPage("/Management/Campaigns");
            }

            // Authorized users must be associated with a valid charity
            if (!CharityId.HasValue || CharityId.Value <= 0)
            {
                return RedirectToPage("/Management/Campaigns");
            }

            // Render the page if all access checks pass
            return Page();
        }
    }
}
