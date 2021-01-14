using System.Threading.Tasks;
using Dapper;
using Microservices.Core.Domain.SeedWork;

namespace Identity.API.Application.Queries.Services
{
    public class AuthQueries : QueryBase, IAuthQueries
    {
        #region Public Constructors

        public AuthQueries(string connectionString) : base(connectionString)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        /*public async Task<Order> GetOrderAsync(int orderId)
        {
            return await WithConnection(async conn =>
            {
                return await conn.QueryFirstOrDefaultAsync<Order>("");
            });
        }*/

        #endregion Public Methods
    }
}