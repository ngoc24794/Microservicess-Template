using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Identity.API.Application.Queries.Models;
using MediatR;
using Microservices.Core.Domain.SeedWork;
using Microservices.Core.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Identity.API.Infrastructures
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDbContextCore, IUnitOfWork
    {
        #region Fields

        private readonly IMediator             _mediator;
        private          IDbContextTransaction _currentTransaction;

        #endregion

        #region Constructors

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        #endregion

        #region IDbContextCore Members

        public IDbContextTransaction GetCurrentTransaction()
        {
            return _currentTransaction;
        }

        public bool HasActiveTransaction => _currentTransaction != null;

        public IExecutionStrategy CreateExecutionStrategy()
        {
            return Database.CreateExecutionStrategy();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion

        #region IUnitOfWork Members

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection. Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single
            // transaction including side effects from the domain event handlers which are using the
            // same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
            // You will need to handle eventual consistency and compensatory actions in case of
            // failures in any of the Handlers.
            await _mediator?.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event
            // Handlers) performed through the DbContext will be committed
            await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        #endregion

        #region Methods

        private void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        #endregion
    }
}