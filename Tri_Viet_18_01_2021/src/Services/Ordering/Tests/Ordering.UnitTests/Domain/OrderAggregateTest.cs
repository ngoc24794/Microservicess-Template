using Ordering.Domain.Models.OrderAggregate;
using Xunit;

namespace Ordering.UnitTests.Domain
{
    public class OrderAggregateTest
    {
        #region Public Methods

        [Fact]
        public void Create_order_item_success()
        {
            //Arrange
            var productId = 1;
            var productName = "FakeProductName";
            var unitPrice = 12;
            var discount = 15;
            var pictureUrl = "FakeUrl";
            var units = 5;

            //Act
            var fakeOrderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);

            //Assert
            Assert.NotNull(fakeOrderItem);
        }

        #endregion Public Methods
    }
}