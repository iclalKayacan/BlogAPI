using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;

namespace BlogAPI.Data
{
    public class BlogDbContext : DbContext
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Blog-Category Relationship
            modelBuilder.Entity<Blog>()
                .HasOne<Category>()
                .WithMany(c => c.Blogs)
                .HasForeignKey("CategoryId");

            // Blog-Tag Relationship (Many-to-Many)
            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.Blogs);

            // Blog-Comment Relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId);
        }
    }
}
