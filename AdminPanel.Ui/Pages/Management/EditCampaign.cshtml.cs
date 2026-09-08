using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class EditCampaignModel : PageModel
    {
        public int CampaignId { get; set; }
        public string UserType { get; set; } = "0";
        public string CharityId { get; set; } = "";

        public IActionResult OnGet(int id)
        {
            if (id <= 0)
            {
                return RedirectToPage("/Management/Campaigns");
            }

            CampaignId = id;

            // دریافت نقش و CharityId کاربر از ادعاهای نشست (Claims)
            UserType = User.FindFirst("UserType")?.Value ?? "0";
            CharityId = User.FindFirst("CharityId")?.Value ?? "";

            return Page();
        }
    }
}
