using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.IntegrationEvents.Events;
using Ordering.API.Application.Queries.Models.OrderAggregate;
using Ordering.Domain.Events;
using Ordering.Domain.Models.BuyerAggregate;
using Ordering.Domain.Models.OrderAggregate;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Core.Domain.IntegrationEventLogs;

namespace Ordering.API.Application.Commands
{
    public class OrdersCommandHandler 
        : IRequestHandler<CreateOrderCommand, bool>,
        IRequestHandler<UpdateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<OrdersCommandHandler> _logger;

        public OrdersCommandHandler(IIntegrationEventService integrationEventService,
                                    IOrderRepository orderRepository,
                                    ILogger<OrdersCommandHandler> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
        {
            // Add Integration event to clean the basket
            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(message.UserId);
            await _integrationEventService.AddAndSaveEventAsync(orderStartedIntegrationEvent);

            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate
            var address = new Address(message.Street, message.City, message.State, message.Country, message.ZipCode);
            var order = new Order(message.UserId, message.UserName, address, message.CardTypeId, message.CardNumber, message.CardSecurityNumber, message.CardHolderName, message.CardExpiration);

            foreach (var item in message.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, item.PictureUrl, item.Units);
            }

            _logger.LogInformation("----- Creating Order - Order: {@Order}", order);

            _orderRepository.Add(order);

            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }

        public Task<bool> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}