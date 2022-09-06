using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
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
            string token = HttpContext?.Session?.GetString("Token") ?? String.Empty;

            if (string.IsNullOrWhiteSpace(token))
                return Redirect("/Login?returnUrl=/");

            if(! await _apiHelper.IsValidToken(token))
                return Redirect("/Login?returnUrl=/");

            _logger.LogInformation($"User authenticated: {HttpContext?.User?.Identity?.IsAuthenticated ?? false}");
            _logger.LogInformation($"Current Session: {HttpContext?.Session.TryGetValue("Token", out _).ToString() ?? ""}");
            return Page();
        }      
        
    }
}