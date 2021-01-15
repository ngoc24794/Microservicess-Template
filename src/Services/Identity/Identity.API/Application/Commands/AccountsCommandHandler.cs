using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Identity.API.Application.Queries.Models;
using IdentityModel.Client;
using MediatR;
using Microservices.Core.Domain.IntegrationEventLogs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.API.Application.Commands
{
    public class AccountsCommandHandler 
        : IRequestHandler<RegisterCommand, bool>, IRequestHandler<LoginCommand, bool>
    {
        #region Fields

        private readonly IIntegrationEventService _integrationEventService;
        private readonly ILogger<AccountsCommandHandler> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Constructors

        public AccountsCommandHandler(IIntegrationEventService integrationEventService,
            UserManager<ApplicationUser> userManager, 
            ILogger<AccountsCommandHandler> logger,
            SignInManager<ApplicationUser> signInManager)
        {
            _integrationEventService = integrationEventService ??
                                       throw new ArgumentNullException(nameof(integrationEventService));
            _userManager = userManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signInManager = signInManager;
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

        #endregion

        #region IRequestHandler<LoginCommand,bool> Members

        public async Task<bool> Handle(LoginCommand model, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Start Login");
            var result = 
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("----- Login GetToken");
                HttpClient httpClient = new HttpClient();
                var identityServerResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = "http://localhost:5000/connect/token",
                    GrantType = "password",
                    //Lấy dữ liệu 
                    //1. ClientID ???? => từ client gửi lên hay lấy từ db thông qua username
                    //2. ClientSecret ????  => từ client gửi lên hay lấy từ db thông qua username
                    //3. Scope => mặc định cái API trả về cho User khi đăng nhập thành công hoặc lấy từ db thông qua username
                    
                    /*ClientId = "ConsoleApp_ClientId",
                    ClientSecret = "secret_for_the_consoleapp",
                    Scope = "ApiName",

                    UserName = username,
                    Password = password.ToSha256()*/
                });
                if (!identityServerResponse.IsError){
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", identityServerResponse.AccessToken);
                    var apiResponse = await client.GetAsync("https://localhost:44328/api/values");
                }
                _logger.LogInformation("----- Login successfull");
                return true;
            }
            
            _logger.LogInformation("----- Login UnSuccessfull");
            return false;
        }
        
        #endregion
        
    }
}