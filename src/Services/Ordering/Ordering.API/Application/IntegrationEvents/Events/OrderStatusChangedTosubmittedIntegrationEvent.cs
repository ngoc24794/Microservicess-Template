using Microservices.Core.EventBus.Events;

namespace Ordering.API.Application.IntegrationEvents.Events
{
    public class OrderStatusChangedToSubmittedIntegrationEvent : IntegrationEvent
    {
        #region Public Constructors

        public OrderStatusChangedToSubmittedIntegrationEvent(int orderId, string orderStatus, string buyerName)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
            BuyerName = buyerName;
        }

        #endregion Public Constructors

        #region Public Properties

        public string BuyerName { get; }
        public int OrderId { get; }
        public string OrderStatus { get; }

        #endregion Public Properties
    }
}