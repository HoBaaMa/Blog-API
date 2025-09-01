using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Data.Seeders
{
    public static class RoleSeeder
    {
        public static void SeedRoles(ModelBuilder modelBuilder)
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137",
                    ConcurrencyStamp = "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "4E204EAB-9CE3-463C-BB1B-4326B11659C5",
                    ConcurrencyStamp = "4E204EAB-9CE3-463C-BB1B-4326B11659C5",
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
