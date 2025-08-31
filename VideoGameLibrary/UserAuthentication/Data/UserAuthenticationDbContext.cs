using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UserAuthentication.Models.Identity;

namespace UserAuthentication.Data
{
    public class UserAuthenticationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public UserAuthenticationDbContext(DbContextOptions<UserAuthenticationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
