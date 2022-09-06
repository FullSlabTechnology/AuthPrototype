using System.Security.Claims;

namespace WebAuth.Models
{
    public class AuthenticatedUser
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
