using Ordering.Domain.Models.BuyerAggregate;
using System;
using Xunit;

namespace Ordering.UnitTests.Domain
{
    public class BuyerAggregateTest
    {
        #region Public Methods

        [Fact]
        public void Create_buyer_item_success()
        {
            //Arrange
            var identity = new Guid().ToString();
            var name = "fakeUser";

            //Act
            var fakeBuyerItem = new Buyer(identity, name);

            //Assert
            Assert.NotNull(fakeBuyerItem);
        }

        #endregion Public Methods
    }
}