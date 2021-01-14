using System;
using System.Net;
using System.Threading.Tasks;
using Identity.API.Application.Commands;
using Identity.API.Application.Queries.Services;
using MediatR;
using Microservices.Core.EventBus.CommandBus.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthsController : ControllerBase
    {
        #region Private Fields

        private readonly IAuthQueries _authQueries;
        private readonly ILogger<AuthsController> _logger;
        private readonly IMediator _mediator;

        #endregion Private Fields

        #region Public Constructors

        public AuthsController(IAuthQueries authQueries, ILogger<AuthsController> logger, IMediator mediator)
        {
            _authQueries = authQueries;
            _logger = logger;
            _mediator = mediator;
        }

        #endregion Public Constructors

        #region Public Methods

        /*[Route("{orderId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrderAsync(int orderId)
        {
            var order = await _authQueries.GetOrderAsync(orderId);
            return Ok(order);
        }*/

        [Route("register")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> RegisterAsync([FromBody] RegisterCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var result = false;
            /*if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var identifiedCommand = new IdentifiedCommand<RegisterCommand, bool>(command, guid);
                result = await _mediator.Send(identifiedCommand);
            }*/
            result = await _mediator.Send(command);
            return Ok(result);
        }


        #endregion Public Methods
    }
}