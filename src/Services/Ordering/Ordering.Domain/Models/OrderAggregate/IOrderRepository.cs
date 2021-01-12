using Microservices.Core.Domain.SeedWork;
using Ordering.Domain.Models.OrderAggregate;

namespace Ordering.API.Application.Queries.Models.OrderAggregate
{
    public interface IOrderRepository : IRepository<Order>
    {
    }
}