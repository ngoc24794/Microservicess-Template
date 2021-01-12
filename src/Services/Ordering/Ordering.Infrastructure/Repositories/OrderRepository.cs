using Microservices.Core.Domain.SeedWork;
using Ordering.API.Application.Queries.Models.OrderAggregate;
using Ordering.Domain.Models.OrderAggregate;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository
        : Repository<Order>, IOrderRepository
    {
        #region Public Constructors

        public OrderRepository(DbContextCore context) : base(context)
        {
        }

        #endregion Public Constructors
    }
}