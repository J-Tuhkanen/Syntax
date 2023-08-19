using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syntax.Core.Models;

namespace Syntax.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserAccount>
    {
        public DbSet<Post> Posts { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Blob> Blobs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Load the virtual user property to the model(s) using UserId property value
            // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/one-to-many
            builder.Entity<Post>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();

            builder.Entity<Comment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .IsRequired();
        }
    }
}
