using Blog_API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog_API.Data.Seeders
{
    public class BlogCategorySeeder
    {
        public static void SeedBlogCategories(ModelBuilder modelBuilder)
        {
            List<BlogCategory> categories = new List<BlogCategory>()
            {
                new BlogCategory {Id = Guid.Parse("63F7EE84-62C1-43A1-ABD7-2D08F8C3D9BE"), Name = "TECHNOLOGY"},
                new BlogCategory {Id = Guid.Parse("807EC475-ADDC-48F6-B712-963D8F9CE788"), Name = "EDUCATION"},
                new BlogCategory {Id = Guid.Parse("79AD67A0-9FC0-44C4-93E3-48421A3C814C"), Name = "BUSINESS"},
                new BlogCategory {Id = Guid.Parse("42DF7A0C-E7AB-4C8F-83EF-A469EEE86BBE"), Name = "LIFESTYLE"},
                new BlogCategory {Id = Guid.Parse("C2985499-A435-4B59-83BF-42539A593DCC"), Name = "HEALTH"},
                new BlogCategory {Id = Guid.Parse("6D8772FC-3234-462C-8D03-160530C32638"), Name = "TRAVEL"},
                new BlogCategory {Id = Guid.Parse("3746F212-985B-49C5-88B7-B18F803C17F8"), Name = "FOOD"},
                new BlogCategory {Id = Guid.Parse("4978AC5F-F634-4DA7-BEE1-C535798CA75B"), Name = "ENTERTAINMENT"},
                new BlogCategory {Id = Guid.Parse("6B12653E-AC69-4C06-922B-81337A62457D"), Name = "SCIENCE"},
                new BlogCategory {Id = Guid.Parse("4BECCFE6-5BC8-4497-9C6C-42340F1E0310"), Name = "SPORTS"},
                new BlogCategory {Id = Guid.Parse("91D5137E-599F-498C-84AA-A78C9D4C908F"), Name = "FINANCE"},
                new BlogCategory {Id = Guid.Parse("8C22C7E2-B967-45FB-9260-E083FFAABF67"), Name = "NEWS"},
                new BlogCategory {Id = Guid.Parse("8321F4F0-B946-4F9A-AAB6-BF3A5696F06D"), Name = "OPINION"}
            };

            modelBuilder.Entity<BlogCategory>().HasData(categories);
        }
    }
}
