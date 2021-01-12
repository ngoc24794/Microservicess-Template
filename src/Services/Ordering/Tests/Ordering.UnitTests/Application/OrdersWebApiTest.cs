using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Ordering.API.Application.Queries.Services;
using Ordering.API.Controllers;
using Ordering.Domain.Models.OrderAggregate;
using System.Threading.Tasks;
using Xunit;

namespace Ordering.UnitTests.Application
{
    public class OrdersWebApiTest
    {
        #region Private Fields

        private readonly Mock<ILogger<OrdersController>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IOrderQueries> _orderQueriesMock;

        #endregion Private Fields

        #region Public Constructors

        public OrdersWebApiTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _orderQueriesMock = new Mock<IOrderQueries>();
            _loggerMock = new Mock<ILogger<OrdersController>>();
        }

        #endregion Public Constructors

        #region Public Methods

        [Fact]
        public async Task Get_order_success()
        {
            //Arrange
            var fakeOrderId = 123;
            var fakeDynamicResult = new Order();
            _orderQueriesMock.Setup(x => x.GetOrderAsync(It.IsAny<int>()))
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var orderController = new OrdersController(_orderQueriesMock.Object, _loggerMock.Object, _mediatorMock.Object);
            var actionResult = await orderController.GetOrderAsync(fakeOrderId) as OkObjectResult;

            //Assert
            Assert.Equal(actionResult.StatusCode, (int)System.Net.HttpStatusCode.OK);
        }

        #endregion Public Methods
    }
}