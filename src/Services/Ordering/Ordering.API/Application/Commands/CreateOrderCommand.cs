using MediatR;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// Lệnh tạo mới đơn hàng
    /// </summary>
    public class CreateOrderCommand : IRequest<bool>
    {
        #region Private Fields

        [DataMember]
        private readonly List<OrderItemDTO> _orderItems;

        #endregion Private Fields

        #region Public Constructors

        public CreateOrderCommand()
        {
            _orderItems = new List<OrderItemDTO>();
        }

        public CreateOrderCommand(List<OrderItemDTO> orderItems, DateTime cardExpiration, string cardHolderName, string cardNumber, string cardSecurityNumber, int cardTypeId, string city, string country, string state, string street, string userId, string userName, string zipCode)
        {
            _orderItems = orderItems;
            CardExpiration = cardExpiration;
            CardHolderName = cardHolderName;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            City = city;
            Country = country;
            State = state;
            Street = street;
            UserId = userId;
            UserName = userName;
            ZipCode = zipCode;
        }

        #endregion Public Constructors

        #region Public Properties

        [DataMember]
        public DateTime CardExpiration { get; private set; }

        [DataMember]
        public string CardHolderName { get; private set; }

        [DataMember]
        public string CardNumber { get; private set; }

        [DataMember]
        public string CardSecurityNumber { get; private set; }

        [DataMember]
        public int CardTypeId { get; private set; }

        [DataMember]
        public string City { get; private set; }

        [DataMember]
        public string Country { get; private set; }

        [DataMember]
        public IEnumerable<OrderItemDTO> OrderItems => _orderItems;

        [DataMember]
        public string State { get; private set; }

        [DataMember]
        public string Street { get; private set; }

        [DataMember]
        public string UserId { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public string ZipCode { get; private set; }

        #endregion Public Properties
    }

    public class OrderItemDTO
    {
        #region Public Properties

        public decimal Discount { get; set; }
        public string PictureUrl { get; set; }
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }
        public int Units { get; set; }

        #endregion Public Properties
    }
}