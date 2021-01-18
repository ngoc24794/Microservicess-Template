using Microservices.Core.Domain.SeedWork;
using System.Threading.Tasks;

namespace Ordering.Domain.Models.BuyerAggregate
{
    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Buyer Aggregate

    public interface IBuyerRepository : IRepository<Buyer>
    {
        Task<Buyer> FindAsync(string BuyerIdentityGuid);
        Task<Buyer> FindByIdAsync(string id);
    }
}