using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAuth.Services;

namespace WebAuth.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IApiHelper _apiHelper;

        public IndexModel(ILogger<IndexModel> logger, IApiHelper apiHelper)
        {
            _logger = logger;
            _apiHelper = apiHelper; 
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string token = HttpContext.Session.GetString("Token");
            if (token is null)
                return Redirect("/Login?returnUrl=/");
            if(! await _apiHelper.IsValidToken(token))
                return Redirect("/Login?returnUrl=/");

            HttpContext.User = await _apiHelper.GetUser();

            return Page();
        }      
        
    }
}