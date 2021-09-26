using DocVault.Models;
using Microsoft.EntityFrameworkCore;

namespace DocVault.DAL
{
    public class DocVaultDbContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DocVaultDbContext(DbContextOptions<DocVaultDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>()
                        .HasIndex(d => d.Checksum);
            modelBuilder.Entity<Document>()
                        .HasIndex(d => d.FileSize);
        }
    }
}