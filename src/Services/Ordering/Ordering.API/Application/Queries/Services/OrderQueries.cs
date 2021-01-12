using Dapper;
using Microservices.Core.Domain.SeedWork;
using Ordering.Domain.Models.OrderAggregate;
using System.Threading.Tasks;

namespace Ordering.API.Application.Queries.Services
{
    public class OrderQueries : QueryBase, IOrderQueries
    {
        #region Public Constructors

        public OrderQueries(string connectionString) : base(connectionString)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<Order> GetOrderAsync(int orderId)
        {
            return await WithConnection(async conn =>
            {
                return await conn.QueryFirstOrDefaultAsync<Order>("");
            });
        }

        #endregion Public Methods
    }
}