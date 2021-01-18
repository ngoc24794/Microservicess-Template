using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Ordering.Infrastructure;

namespace Ordering.API.AutofacModules
{
    public class OrderingContextDesignFactory : IDesignTimeDbContextFactory<OrderingContext>
    {
        #region Public Methods

        public OrderingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>()
                .UseSqlServer("Server=192.168.0.128;Initial Catalog=OrderingDb;User Id=avinams;Password=ABcd1234!@#$");

            return new OrderingContext(optionsBuilder.Options);
        }

        #endregion Public Methods
    }
}