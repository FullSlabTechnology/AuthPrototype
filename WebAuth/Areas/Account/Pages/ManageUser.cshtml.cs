using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAuth.Areas.Account.Pages
{
    [Authorize]
    public class ManageUserModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
