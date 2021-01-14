using System;
using System.Threading;
using System.Threading.Tasks;
using Identity.API.Application.IntegrationEvents.Events;
using Identity.API.Application.Queries.Models;
using MediatR;
using Microservices.Core.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.API.Application.Commands
{
    public class AccountsCommandHandler 
        : IRequestHandler<RegisterCommand, bool>
    {
        private readonly IIntegrationEventService _integrationEventService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountsCommandHandler> _logger;

        public AccountsCommandHandler(IIntegrationEventService integrationEventService,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountsCommandHandler> logger)
        {
            _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
            _userManager = userManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(RegisterCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Start Register");
            //Xử lý dữ liệu đăng ký
            var user = new ApplicationUser() {
                UserName = message.Email, 
                Email = message.Email 
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
    }
}