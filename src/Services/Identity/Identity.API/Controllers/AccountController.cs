using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Identity.API.Application.Commands;
using Identity.API.Application.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

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
        
        [Route("register_from_excel")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> RegisterFromExcelAsync([FromBody] RegisterFromExcelCommand command, IFormFile formFile)
        {
            /*if (formFile == null || formFile.Length <= 0)
            {
                return false;
            }  
  
            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }  
  
            var list = new List<RegisterFromExcel>();

            await using (var stream = new MemoryStream())  
            {  
                await formFile.CopyToAsync(stream, cancellationToken);  
  
                using (var package = new ExcelPackage(stream))  
                {  
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];  
                    var rowCount = worksheet.Dimension.Rows;  
  
                    for (int row = 2; row <= rowCount; row++)  
                    {  
                        list.Add(new RegisterFromExcel  
                        {  
                            STT = Convert.ToInt32(worksheet.Cells[row, 1].Value.ToString().Trim()),  
                            Age = int.Parse(worksheet.Cells[row, 2].Value.ToString().Trim()),  
                        });  
                    }  
                }  
            }  */
            
            return Ok(await _mediator.Send(command));
        }

        [Route("logout")]
        [HttpPost]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> LogoutAsync([FromBody] LogoutCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        #endregion
    }
}