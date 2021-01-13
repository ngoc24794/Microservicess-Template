using MediatR;
using Microservices.Core.EventBus.Abstractions;
using Microservices.Core.EventBus.CommandBus.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ordering.API.Application.Commands;
using Ordering.API.Application.Queries.Services;
using Ordering.Domain.Models.OrderAggregate;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        #region Private Fields

        private readonly IOrderQueries _orderQueries;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMediator _mediator;

        #endregion Private Fields

        #region Public Constructors

        public OrdersController(IOrderQueries orderQueries, ILogger<OrdersController> logger, IMediator mediator)
        {
            _orderQueries = orderQueries;
            _logger = logger;
            _mediator = mediator;
        }

        #endregion Public Constructors

        #region Public Methods

        [Route("{orderId:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(Order), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetOrderAsync(int orderId)
        {
            var order = await _orderQueries.GetOrderAsync(orderId);
            return Ok(order);
        }

        [Route("order")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> CreateOrderFromBasketDataAsync([FromBody] CreateOrderCommand command, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var result = false;
            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                var identifiedCommand = new IdentifiedCommand<CreateOrderCommand, bool>(command, guid);
                result = await _mediator.Send(identifiedCommand);
            }
            return Ok(result);
        }


        #endregion Public Methods
    }
}