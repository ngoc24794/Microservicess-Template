using Ordering.Domain.Models.OrderAggregate;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries.Services
{
    public interface IOrderQueries
    {
        #region Public Methods

        Task<Order> GetOrderAsync(int orderId);

        #endregion Public Methods
    }
}