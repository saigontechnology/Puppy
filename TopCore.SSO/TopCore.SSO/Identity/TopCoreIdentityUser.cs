using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TopCore.SSO.Identity
{
    public class TopCoreIdentityUser : IdentityUser
    {
        public TopCoreIdentityUser()
        {
        }

        public TopCoreIdentityUser(string userName) : base(userName)
        {
        }

        public string Password { get; set; }
    }
}