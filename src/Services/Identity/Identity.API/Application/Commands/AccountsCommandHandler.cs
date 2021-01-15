using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.API.Application.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.API.Application.Commands
{
    public class AccountsCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        #region Fields

        private readonly ILogger<AccountsCommandHandler> _logger;
        private readonly UserManager<ApplicationUser>    _userManager;

        #endregion

        #region Constructors

        public AccountsCommandHandler(
            UserManager<ApplicationUser>    userManager,
            ILogger<AccountsCommandHandler> logger)
        {
            _userManager = userManager ?? throw new ArgumentException(nameof(userManager));
            _logger      = logger      ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region IRequestHandler<RegisterCommand,bool> Members

        public async Task<bool> Handle(RegisterCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Start Register");
            //Xử lý dữ liệu đăng ký
            var user = new ApplicationUser
            {
                UserName = message.Email,
                Email    = message.Email
            };
            var result = await _userManager.CreateAsync(user, message.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                return true;
            }

            _logger.LogInformation("--- Register UnSuccessfull");
            return false;
        }

        #endregion
    }
}