using MediatR;
using Microservices.Core.Domain.Idempotency;
using Microservices.Core.Domain.SeedWork;
using Microservices.Core.IntegrationEventLogEF;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Models.BuyerAggregate;
using Ordering.Domain.Models.OrderAggregate;
using Ordering.Infrastructure.EntityConfigurations;

namespace Ordering.Infrastructure
{
    public class OrderingContext : DbContextCore, IUnitOfWork
    {
        #region Public Constructors

        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {
        }

        public OrderingContext(DbContextOptions<OrderingContext> options, IMediator mediator) : base(options, mediator)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }
        public DbSet<PaymentMethod> Payments { get; set; }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientRequestEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new IntegrationEventLogEntryTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerEntityTypeConfiguration());
        }

        #endregion Protected Methods
    }
}