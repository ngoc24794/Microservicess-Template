using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Ordering.API.Application.Commands;
using Ordering.API.Application.Queries.Models.OrderAggregate;
using Ordering.Domain.Models.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microservices.Core.Domain.IntegrationEventLogs;
using Xunit;
using static Ordering.API.Application.Commands.CreateOrderCommand;

namespace Ordering.UnitTests.Application
{
    public class OrdersCommandHandlerTest
    {
        #region Private Fields

        private readonly Mock<IIntegrationEventService> _integrationEventService;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;

        #endregion Private Fields

        #region Public Constructors

        public OrdersCommandHandlerTest()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _integrationEventService = new Mock<IIntegrationEventService>();
        }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task Handle_return_false_if_order_is_not_persisted()
        {
            var fakeOrderCmd = FakeOrderRequestWithBuyer(new Dictionary<string, object>
            { ["cardExpiration"] = DateTime.Now.AddYears(1) });

            _orderRepositoryMock.Setup(r => r.UnitOfWork.SaveChangesAsync(default))
                .Returns(Task.FromResult(1));

            var loggerMock = new Mock<ILogger<OrdersCommandHandler>>();

            //Act
            var handler = new OrdersCommandHandler(_integrationEventService.Object, _orderRepositoryMock.Object, loggerMock.Object);
            var cltToken = new CancellationToken();
            var result = await handler.Handle(fakeOrderCmd, cltToken);

            //Assert
            Assert.False(result);
        }

        #endregion Public Methods

        #region Private Methods

        private CreateOrderCommand FakeOrderRequestWithBuyer(Dictionary<string, object> args = null)
        {
            return new CreateOrderCommand(
                new List<OrderItemDTO>(),
                userId: args != null && args.ContainsKey("userId") ? (string)args["userId"] : null,
                userName: args != null && args.ContainsKey("userName") ? (string)args["userName"] : null,
                city: args != null && args.ContainsKey("city") ? (string)args["city"] : null,
                street: args != null && args.ContainsKey("street") ? (string)args["street"] : null,
                state: args != null && args.ContainsKey("state") ? (string)args["state"] : null,
                country: args != null && args.ContainsKey("country") ? (string)args["country"] : null,
                zipCode: args != null && args.ContainsKey("zipcode") ? (string)args["zipcode"] : null,
                cardNumber: args != null && args.ContainsKey("cardNumber") ? (string)args["cardNumber"] : "1234",
                cardExpiration: args != null && args.ContainsKey("cardExpiration") ? (DateTime)args["cardExpiration"] : DateTime.MinValue,
                cardSecurityNumber: args != null && args.ContainsKey("cardSecurityNumber") ? (string)args["cardSecurityNumber"] : "123",
                cardHolderName: args != null && args.ContainsKey("cardHolderName") ? (string)args["cardHolderName"] : "XXX",
                cardTypeId: args != null && args.ContainsKey("cardTypeId") ? (int)args["cardTypeId"] : 0);
        }

        #endregion Private Methods
    }
}