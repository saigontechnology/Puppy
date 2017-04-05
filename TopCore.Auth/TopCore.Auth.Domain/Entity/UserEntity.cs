
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TopCore.Auth.Domain.Entity
{
    public class UserEntity : IdentityUser
    {
        public UserEntity()
        {
        }

        public UserEntity(string userName) : base(userName)
        {
        }

        public string Password { get; set; }
    }
}