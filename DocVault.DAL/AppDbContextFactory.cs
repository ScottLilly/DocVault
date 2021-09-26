using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DocVault.DAL
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<DocVaultDbContext>
    {
        public DocVaultDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DocVaultDbContext>();
            optionsBuilder.UseSqlServer("Server=.;Initial Catalog=DocVault;Trusted_Connection=True;Connection Timeout=30;");

            return new DocVaultDbContext(optionsBuilder.Options);
        }
    }
}