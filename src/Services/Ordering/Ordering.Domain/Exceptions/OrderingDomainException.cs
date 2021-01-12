using System;

namespace Ordering.Domain.Exceptions
{
    public class OrderingDomainException : Exception
    {
        public OrderingDomainException(string message) : base(message)
        {
        }
    }
}