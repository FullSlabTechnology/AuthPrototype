using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebAuth.Services;

namespace WebAuth.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {

        private readonly IApiHelper _apiHelper;

        [Required]
        [BindProperty]
        public string UserName { get; set; }

        [Required]
        [BindProperty]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }
        [TempData]
        public string? ErrorMessage { get; set; }


        public LoginModel(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public void OnGet(string returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = "/")
        {
            ReturnUrl = returnUrl;
            try
            {


                var token = await _apiHelper.Authenticate(UserName, Password);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    HttpContext.Session.SetString("Token", token);
                    return LocalRedirect(Url.GetLocalUrl(ReturnUrl));
                }
                else
                    throw new NullReferenceException("Authenticate result was null");

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return Page();
        }
    }
}
