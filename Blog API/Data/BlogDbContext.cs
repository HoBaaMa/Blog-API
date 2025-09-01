using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Blog_API.Models.Entities;
using Blog_API.Data.Seeders;

namespace Blog_API.Data
{
    public class BlogDbContext : IdentityDbContext<ApplicationUser>
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Tag> Tags { get; set; }
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

                e.Property(e => e.BlogCategory)
                .HasConversion<string>();

                e.Property(e => e.ImageUrls)
                .HasConversion(
                    v => string.Join(';', v), // Convert list to string
                    v => v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList()) // Convert string to list
                .HasColumnName("ImageUrls");

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

                e.HasOne(e => e.ParentComment)
                .WithMany(e => e.Replies)
                .HasForeignKey(e => e.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<Like>(l =>
            {
                l.HasKey(l => l.Id);

                l.Property(l => l.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

                l.HasOne(l => l.User)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                l.HasOne(l => l.BlogPost)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.BlogPostId)
                .OnDelete(DeleteBehavior.Restrict);

                l.HasOne(l => l.Comment)
                .WithMany(l => l.Likes)
                .HasForeignKey(l => l.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

                l.HasIndex(e => new { e.UserId, e.BlogPostId }).IsUnique();
                l.HasIndex(e => new { e.UserId, e.CommentId }).IsUnique();

            });

            modelBuilder.Entity<Tag>(t =>
            {
                t.HasKey(t => t.Id);

                t.HasMany(t => t.BlogPosts)
                .WithMany(t => t.Tags);

                //t.Property(t => t.Name)
                //.HasMaxLength(25)
                //.IsRequired();
            });


            // Seeding Roles
            RoleSeeder.SeedRoles(modelBuilder);

            // Seeding Users
            UserSeeder.SeedUsers(modelBuilder);
        }
    }
}
