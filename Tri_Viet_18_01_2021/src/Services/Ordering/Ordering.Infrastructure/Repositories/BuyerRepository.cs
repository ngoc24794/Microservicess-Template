using Microservices.Core.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Models.BuyerAggregate;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Repositories
{
    public class BuyerRepository
        : Repository<Buyer>, IBuyerRepository
    {
        #region Public Constructors

        public BuyerRepository(DbContextCore context) : base(context)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<Buyer> FindAsync(string identity)
        {
            var buyer = await (_context as OrderingContext).Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.IdentityGuid == identity)
                .SingleOrDefaultAsync();

            return buyer;
        }

        public async Task<Buyer> FindByIdAsync(string id)
        {
            var buyer = await (_context as OrderingContext).Buyers
                .Include(b => b.PaymentMethods)
                .Where(b => b.Id == int.Parse(id))
                .SingleOrDefaultAsync();

            return buyer;
        }

        #endregion Public Methods
    }
}