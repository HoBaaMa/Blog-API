using Blog_API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Data.Seeders
{
    public static class UserSeeder
    {
        public static void SeedUsers(ModelBuilder modelBuilder)
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2",
                ConcurrencyStamp = "5F4AA7CD-65EB-4821-B950-6D07B3C213FE",
                SecurityStamp = "71E0A7E3-54EC-4757-ACEA-6141748F4C05",
                Email = "admin@xvibes.com",
                NormalizedEmail = "ADMIN@XVIBES.COM",
                EmailConfirmed = true,
                UserName = "Admin",
                NormalizedUserName = "ADMIN" 
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "XVibesAdmin");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137",
                    UserId = "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2"
                });

            var defaultUser = new ApplicationUser
            {
                Id = "EC110106-E170-4C57-9EB3-1697F278E6E7",
                ConcurrencyStamp = "479B7E50-DCB4-4000-ADFA-7D755E946CD5",
                SecurityStamp = "F2FCAB9D-87F2-4CAD-A02E-685C9C4B5DF3",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM", 
                EmailConfirmed = true,
                UserName = "Default User",
                NormalizedUserName = "DEFAULT USER"
            };

            defaultUser.PasswordHash = hasher.HashPassword(defaultUser, "XVibesDUser");

            modelBuilder.Entity<ApplicationUser>().HasData(defaultUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "4E204EAB-9CE3-463C-BB1B-4326B11659C5",
                    UserId = "EC110106-E170-4C57-9EB3-1697F278E6E7"
                });
        }
    }
}
