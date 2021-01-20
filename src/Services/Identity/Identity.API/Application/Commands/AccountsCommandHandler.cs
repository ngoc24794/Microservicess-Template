using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Identity.API.Application.Queries.Models;
using IdentityModel.Client;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Identity.API.Application.Commands
{
    public class AccountsCommandHandler :
        IRequestHandler<RegisterCommand, bool>,
        IRequestHandler<RegisterFromExcelCommand, bool>,
        IRequestHandler<LoginCommand, bool>,
        IRequestHandler<LogoutCommand, bool>
    {
        #region Fields

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountsCommandHandler> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;

        #endregion

        #region Constructors

        public AccountsCommandHandler(
            UserManager<ApplicationUser> userManager,
            ILogger<AccountsCommandHandler> logger,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _hostingEnvironment = hostingEnvironment;
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

        #region IRequestHandler<RegisterFromExcelCommand,bool> Members

        public async Task<bool> Handle(RegisterFromExcelCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("----- Start require excel file");
            /*string path = _hostingEnvironment.MapPath("~/Resources/register_template.xlsx");*/
            /*string webRootPath = _hostingEnvironment.WebRootPath;
            string path = "D:\\Tri_Viet\\src\\Services\\Identity\\Identity.API\\Resources\\register_template.xlsx";*/
                //Path.Combine(webRootPath + "/Identity.API/Resources/register_template.xlsx");

            /*FileInfo fileInfo = new FileInfo(path);

            ExcelPackage package = new ExcelPackage(fileInfo);
            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

// get number of rows and columns in the sheet
            if (worksheet != null)
            {
                int rows = worksheet.Dimension.Rows; // 20
                int columns = worksheet.Dimension.Columns; // 7

// loop through the worksheet rows and columns
                for (int i = 1; i <= rows; i++)
                {
                    for (int j = 1; j <= columns; j++)
                    {
                        string content = worksheet.Cells[i, j].Value.ToString();
                        /* Do something ...#1#
                    }
                }
            }*/

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
                    _httpContextAccessor.HttpContext.Response.Cookies.Append("ckToken", tokenReturn.AccessToken,
                        option);

                    return true;
                }

                _logger.LogInformation("----- Login successfull");
                return true;
            }

            _logger.LogInformation("----- Login UnSuccessfull");
            return false;
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