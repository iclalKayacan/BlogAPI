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

            // Blog-Category Many-to-Many Relationship
            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Blogs);

            // Blog-Tag Many-to-Many Relationship
            modelBuilder.Entity<Blog>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.Blogs);

            // Blog-Comment Relationship
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlogId);
        }
        public override int SaveChanges()
        {
            // CreatedAt ve UpdatedAt otomatik ayarı
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Blog blog)
                {
                    if (entry.State == EntityState.Added)
                    {
                        blog.CreatedAt = DateTime.Now;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        blog.UpdatedAt = DateTime.Now;
                    }
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Blog blog)
                {
                    if (entry.State == EntityState.Added)
                    {
                        blog.CreatedAt = DateTime.Now;
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        blog.UpdatedAt = DateTime.Now;
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
