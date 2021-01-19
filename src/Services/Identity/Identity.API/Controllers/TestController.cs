using System.Net;
using System.Threading.Tasks;
using Identity.API.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [Route("au")]
        [HttpGet]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        public ActionResult AuAsync()
        {
            return Ok("true");
        }
    }
}