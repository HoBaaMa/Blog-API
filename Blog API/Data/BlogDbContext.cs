using Blog_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Blog_API.Data
{
    public class BlogDbContext : IdentityDbContext<ApplicationUser>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<BlogPost> blogPosts { get; set; }
        public DbSet<Comment> comments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<BlogPost>(e =>
            {
                e.HasKey(e => e.Id);

                e.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

                e.Property(e => e.Content)
                .IsRequired();

                e.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

                e.HasOne(e => e.User)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<Comment>(e =>
            {
                e.HasKey(e => e.Id);

                e.Property(e => e.Content)
                .IsRequired();

                e.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

                e.HasOne(e => e.User)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

                e.HasOne(e => e.BlogPost)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict);


            });
        }
    }
}
