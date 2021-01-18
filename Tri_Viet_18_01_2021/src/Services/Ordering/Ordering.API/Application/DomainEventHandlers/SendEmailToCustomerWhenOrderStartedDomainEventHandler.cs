using MediatR;
using Ordering.Domain.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers
{
    public class SendEmailToCustomerWhenOrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        #region Public Constructors

        public SendEmailToCustomerWhenOrderStartedDomainEventHandler()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion Public Methods
    }
}