using MediatR;
using Microservices.Core.EventBus.CommandBus.Handling;
using Microservices.Core.EventBus.CommandBus.Idempotency;
using Microsoft.Extensions.Logging;

namespace Ordering.API.Application.Commands
{
    public class CreateOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CreateOrderCommand, bool>
    {
        #region Public Constructors

        public CreateOrderIdentifiedCommandHandler(IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<CreateOrderCommand, bool>> logger) : base(mediator, requestManager, logger)
        {
        }

        #endregion Public Constructors
    }
}