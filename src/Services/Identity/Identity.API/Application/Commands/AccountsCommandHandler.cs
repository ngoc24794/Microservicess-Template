using System;
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
        private readonly IdentityServerTools _identityServerTools;
        private readonly UserManager<ApplicationUser> _userManager;

        #endregion

        #region Constructors

        public AccountsCommandHandler(IIntegrationEventService integrationEventService,
            UserManager<ApplicationUser> userManager, 
            ILogger<AccountsCommandHandler> logger,
            SignInManager<ApplicationUser> signInManager,
            IdentityServerTools identityServerTools)
        {
            _integrationEventService = integrationEventService ??
                                       throw new ArgumentNullException(nameof(integrationEventService));
            _userManager = userManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signInManager = signInManager;
            _identityServerTools = identityServerTools;
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
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("----- Login GetToken");

                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
                if (disco.IsError)
                {
                    _logger.LogWarning("Không tìm thấy IP");
                }
                else
                {
                    // request token
                    /*var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = "client",
                        ClientSecret = "secret",
                        Scope = "api1"
                    });*/

                    
                    var tokenReturn=await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        Password = model.Password,//.ToSha256(),
                        ClientId = "ndev01",
                        UserName = model.Email,
                        ClientSecret = "secret",
                        Scope = "apiNDEVpp",
                        GrantType = "password",
                        
                    });
                    string ketqa = "";
                    /*var token2 = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
                    {
                        Address = disco.TokenEndpoint,
                        ClientId = "mvc",
                        ClientSecret = "secret",
                        GrantType = "authorization_code"
                    });*/

                    /*var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        Address = disco.TokenEndpoint,

                        ClientId = "client",
                        ClientSecret = "secret",
                        Scope = "api1"
                    });

                    if (tokenResponse.IsError)
                    {
                        Console.WriteLine(tokenResponse.Error);
                        return;
                    }*/
                }
                
                /*HttpClient httpClient = new HttpClient();
                /*var identityServerResponse = await httpClient.RequestPasswordTokenAsync(new PasswordTokenRequest#1#
                var identityServerResponse = await httpClient.requ(new PasswordTokenRequest
                {
                    Address = "http://localhost:5000/connect/token",
                    ClientId = "client",
                    UserName = model.Email,
                    Password = model.Password,
                    GrantType = "password",//"client_credentials",
                    ClientSecret = "secret",
                    Scope = "api1"

                    //Lấy dữ liệu 
                    //1. ClientID ???? => từ client gửi lên hay lấy từ db thông qua username
                    //2. ClientSecret ????  => từ client gửi lên hay lấy từ db thông qua username
                    //3. Scope => mặc định cái API trả về cho User khi đăng nhập thành công hoặc lấy từ db thông qua username

                    /*ClientId = "ConsoleApp_ClientId",
                    ClientSecret = "secret_for_the_consoleapp",
                    Scope = "ApiName",

                    UserName = username,
                    Password = password.ToSha256()#1#
                });
                if (!identityServerResponse.IsError){
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", identityServerResponse.AccessToken);
                   //var apiResponse = await client.GetAsync("https://localhost:44328/api/values");
                }
                var issuer = "http://" + httpRequest.Host.Value;  
                var token = await _identityServerTools.IssueJwtAsync(  
                    30000,  
                    issuer,  
                    new System.Security.Claims.Claim[1]   
                    {  
                        new System.Security.Claims.Claim("cpf", cpf)  
                    }  
                );*/
                _logger.LogInformation("----- Login successfull");
                return true;
            }
            
            _logger.LogInformation("----- Login UnSuccessfull");
            return false;
        }
        
        #endregion
        
    }
}