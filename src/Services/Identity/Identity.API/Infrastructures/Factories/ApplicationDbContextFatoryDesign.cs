using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.API.Infrastructures.Factories
{
    public class ApplicationDbContextFatoryDesign : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        #region IDesignTimeDbContextFactory<ApplicationDbContext> Members

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer("Server=192.168.0.128;Initial Catalog=IdentityDb;User Id=avinams;Password=ABcd1234!@#$");

            return new ApplicationDbContext(optionsBuilder.Options, null);
        }

        #endregion
    }
}