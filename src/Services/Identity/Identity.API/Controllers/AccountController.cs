using System.Net;
using System.Threading.Tasks;
using Identity.API.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        #region Fields

        private readonly ILogger<AccountController> _logger;
        private readonly IMediator _mediator;

        #endregion

        #region Constructors

        public AccountController(ILogger<AccountController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        #endregion

        #region Methods

        [Route("login")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> LoginAsync([FromBody] LoginCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        
        [Route("register")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> RegisterAsync([FromBody] RegisterCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [Route("logout")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> LogoutAsync([FromBody] LogoutCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [Route("au")]
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> AuAsync()
        {
            return Content("OK");
        }
        [Route("test")]
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<bool>> TestAsync([FromBody] TestCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        #endregion
    }
}