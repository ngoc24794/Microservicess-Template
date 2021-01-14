using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Infrastructures.Factories
{
    public class ApplicationDbContextFatoryDesign : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseSqlServer("Server=192.168.0.128;Initial Catalog=IdentityDb;User Id=avinams;Password=ABcd1234!@#$");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
