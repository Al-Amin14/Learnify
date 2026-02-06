using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Learnify.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "a1111111-1111-1111-1111-111111111111",
                    Name = "admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "11111111-1111-1111-1111-111111111111"
                },
                new IdentityRole
                {
                    Id = "b2222222-2222-2222-2222-222222222222",
                    Name = "client",
                    NormalizedName = "CLIENT",
                    ConcurrencyStamp = "22222222-2222-2222-2222-222222222222"
                },
                new IdentityRole
                {
                    Id = "c3333333-3333-3333-3333-333333333333",
                    Name = "seller",
                    NormalizedName = "SELLER",
                    ConcurrencyStamp = "33333333-3333-3333-3333-333333333333"
                }
            );
        }
    }
}
