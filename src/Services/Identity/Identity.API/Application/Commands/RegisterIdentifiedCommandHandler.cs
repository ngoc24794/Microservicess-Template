using MediatR;
using Microservices.Core.EventBus.CommandBus.Handling;
using Microservices.Core.EventBus.CommandBus.Idempotency;
using Microsoft.Extensions.Logging;

namespace Identity.API.Application.Commands
{
    public class RegisterIdentifiedCommandHandler : IdentifiedCommandHandler<RegisterCommand, bool>
    {
        #region Public Constructors

        public RegisterIdentifiedCommandHandler(
            IMediator mediator, IRequestManager requestManager, ILogger<IdentifiedCommandHandler<RegisterCommand, bool>> logger) : base(mediator, requestManager, logger)
        {
        }

        #endregion Public Constructors
    }
}