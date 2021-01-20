using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Identity.API.Application.Commands;
using Identity.API.Infrastructures.Models.ExcelRegisterUserModel;
using Identity.API.Infrastructures.Services;
using IdentityServer4.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Register all users from the excel document, return any users that failed to register
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("register-users-excel")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<RegisterListUserResponse<List<ExcelRegisterUserModel>>> Import(IFormFile formFile, CancellationToken cancellationToken)
        {
            var list = new List<ExcelRegisterUserModel>();

            if (formFile == null || formFile.Length <= 0)
            {
                return RegisterListUserResponse<List<ExcelRegisterUserModel>>.GetResult("No file uploaded", list);
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return RegisterListUserResponse<List<ExcelRegisterUserModel>>.GetResult("Not Support file extension", list);
            }

            await using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream, cancellationToken);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        list.Add(new ExcelRegisterUserModel
                        {
                            UserName = worksheet.Cells[row, 1].Value.ToString()?.Trim(),
                            Password = worksheet.Cells[row, 2].Value.ToString()?.Trim(),
                            Email = worksheet.Cells[row, 3].Value.ToString()?.Trim(),
                            EmailConfirmed = Convert.ToBoolean(worksheet.Cells[row, 4].Value.ToString() ?? "false"),
                        });
                    }

                    RegisterListUsersCommand command = new RegisterListUsersCommand(list);
                    list = await _mediator.Send(command);
                }
            }
            
            return RegisterListUserResponse<List<ExcelRegisterUserModel>>.GetResult(list.IsNullOrEmpty() ? "Registered" : "Register UnSuccessful", list);
        }
        #endregion
    }
}