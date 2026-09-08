using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Ui.Pages.Management
{
    public class UsersModel : PageModel
    {
        public void OnGet()
        {
            // No special server-side logic is needed for now, since the data is loaded via AJAX.
        }
    }
}
