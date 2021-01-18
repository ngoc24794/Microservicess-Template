using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasketController : ControllerBase
    {
        #region Methods

        // GET
        [HttpGet]
        // [Authorize(Policy = "Api1Policy")]
        public IActionResult Get()
        {
            return Ok("Hello");
        }

        #endregion
    }
}