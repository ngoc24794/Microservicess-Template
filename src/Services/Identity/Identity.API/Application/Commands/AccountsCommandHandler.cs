using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Identity.API.Application.Queries.Models;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using MediatR;
using Microservices.Core.Domain.IntegrationEventLogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.API.Application.Commands
{
    public class AccountsCommandHandler :
        IRequestHandler<RegisterCommand, bool>,
        IRequestHandler<LoginCommand, bool>,
        IRequestHandler<TestCommand, bool>
        IRequestHandler<LoginCommand, bool>,
        IRequestHandler<LogoutCommand, bool>
    {
        #region Fields

        private readonly IIntegrationEventService _integrationEventService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountsCommandHandler> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IdentityServerTools _identityServerTools;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Constructors

        public AccountsCommandHandler(IIntegrationEventService integrationEventService,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountsCommandHandler> logger,
            SignInManager<ApplicationUser> signInManager,
            IdentityServerTools identityServerTools,
            IHttpContextAccessor httpContextAccessor)
        {
            _integrationEventService = integrationEventService ??
                                       throw new ArgumentNullException(nameof(integrationEventService));
            _userManager = userManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signInManager = signInManager;
            _identityServerTools = identityServerTools;
            _httpContextAccessor = httpContextAccessor;
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
                Email = message.Email,
                EmailConfirmed = false
            };
            var result = await _userManager.CreateAsync(user, message.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password");
                return true;
            }

            _logger.LogInformation("--- Register UnSuccessfull");
            return false;
        }

        #endregion

        #region IRequestHandler<LoginCommand,bool> Members

        public async Task<bool> Handle(LoginCommand model, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Start Login");
            var result =
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember,
                    lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("----- Login GetToken");

                var client = new HttpClient();
                var disco =
                    await client.GetDiscoveryDocumentAsync("http://localhost:5000",
                        cancellationToken: cancellationToken);
                if (disco.IsError)
                {
                    _logger.LogWarning("IP Not found");
                }
                else
                {
                    _logger.LogInformation("----- Login create accesstoken");
                    var tokenReturn = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        Password = model.Password,
                        ClientId = "ndev01",
                        UserName = model.Email,
                        ClientSecret = "secret",
                        Scope = "TEST",
                        GrantType = "password",
                    }, cancellationToken: cancellationToken);

                    _logger.LogInformation("----- Login create cookie (ckToken)");
                    CookieOptions option = new CookieOptions
                    {
                        Expires = DateTime.Now.AddHours(4)
                    };
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("ckToken", tokenReturn.AccessToken, option);
                    return true;
                }

                _logger.LogInformation("----- Login successfull");
                return true;
            }

            _logger.LogInformation("----- Login UnSuccessfull");
            return false;
        }

        public async Task<bool> Handle(TestCommand message, CancellationToken cancellationToken)
        {
            return true;
        }
        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
             await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
             //đang update phần return url
            return true; 
        }

        #endregion
        
    }
}