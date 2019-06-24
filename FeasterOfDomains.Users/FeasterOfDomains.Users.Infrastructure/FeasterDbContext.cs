namespace FeasterOfDomains.Users.Infrastructure
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    public class FeasterDbContext : IdentityDbContext<FeasterUser>
    {
        public FeasterDbContext(DbContextOptions<FeasterDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}