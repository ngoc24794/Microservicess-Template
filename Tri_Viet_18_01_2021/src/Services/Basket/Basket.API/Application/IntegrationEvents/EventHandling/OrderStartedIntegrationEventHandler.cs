using Microservices.Core.EventBus.Abstractions;
using System.Threading.Tasks;

namespace Basket.API.Application.IntegrationEvents.EventHandling
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}