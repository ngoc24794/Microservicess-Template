using Microservices.Core.EventBus.Events;

namespace Basket.API.Application.IntegrationEvents.EventHandling
{
    // Integration Events notes: An Event is “something that has happened in the past”, therefore
    // its name has to be An Integration Event is an event that can cause side effects to other
    // microsrvices, Bounded-Contexts or external systems.
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        #region Public Constructors

        public OrderStartedIntegrationEvent(string userId) => UserId = userId;

        #endregion Public Constructors

        #region Public Properties

        public string UserId { get; set; }

        #endregion Public Properties
    }
}