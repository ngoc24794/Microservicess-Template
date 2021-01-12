using MediatR;
using Microservices.Core.Domain.Services;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.Domain.Events;
using Ordering.Domain.Models.BuyerAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.DomainEventHandlers
{
    public class ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler
                        : INotificationHandler<OrderStartedDomainEvent>
    {
        #region Private Fields

        private readonly IBuyerRepository _buyerRepository;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> _logger;

        #endregion Private Fields

        #region Public Constructors

        public ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler(ILogger<ValidateOrAddBuyerAggregateWhenOrderStartedDomainEventHandler> logger,
                                                                             IBuyerRepository buyerRepository,
                                                                             IIntegrationEventService integrationEventService)
        {
            _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task Handle(OrderStartedDomainEvent orderStartedEvent, CancellationToken cancellationToken)
        {
            var cardTypeId = orderStartedEvent.CardTypeId != 0 ? orderStartedEvent.CardTypeId : 1;
            var buyer = await _buyerRepository.FindAsync(orderStartedEvent.UserId);
            bool buyerOriginallyExisted = buyer == null ? false : true;

            if (!buyerOriginallyExisted)
            {
                buyer = new Buyer(orderStartedEvent.UserId, orderStartedEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                           $"Payment Method on {DateTime.UtcNow}",
                                           orderStartedEvent.CardNumber,
                                           orderStartedEvent.CardSecurityNumber,
                                           orderStartedEvent.CardHolderName,
                                           orderStartedEvent.CardExpiration,
                                           orderStartedEvent.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ?
                _buyerRepository.Update(buyer) :
                _buyerRepository.Add(buyer);

            await _buyerRepository.UnitOfWork
                .SaveEntitiesAsync(cancellationToken);

            var orderStatusChangedTosubmittedIntegrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(orderStartedEvent.Order.Id, orderStartedEvent.Order.OrderStatus.Name, buyer.Name);
            await _integrationEventService.AddAndSaveEventAsync(orderStatusChangedTosubmittedIntegrationEvent);
            _logger.LogTrace("Buyer {BuyerId} and related payment method were validated or updated for orderId: {OrderId}.", buyerUpdated.Id, orderStartedEvent.Order.Id);
        }

        #endregion Public Methods
    }
}