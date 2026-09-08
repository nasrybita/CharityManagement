using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class CampaignDetailsModel : PageModel
    {
        public int CampaignId { get; set; }
        public bool IsSystemAdmin { get; set; }

        public void OnGet(int id)
        {
            CampaignId = id;


            //User role extraction from claims
            var userType = User.FindFirst("UserType")?.Value;
            IsSystemAdmin = userType == "1";
        }
    }
}
