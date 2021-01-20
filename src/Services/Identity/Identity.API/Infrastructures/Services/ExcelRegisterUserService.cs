using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Application.Queries.Models;
using Identity.API.Infrastructures.Models.ExcelRegisterUserModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.API.Infrastructures.Services
{
    public class ExcelRegisterUserService : IExcelRegisterUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExcelRegisterUserService> _logger;

        public ExcelRegisterUserService(UserManager<ApplicationUser> userManager,
            ILogger<ExcelRegisterUserService> logger)
        {
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// Register all users from the list, return any users that failed to register
        /// </summary>
        /// <param name="users"></param>
        /// <returns>Register unsuccessful users</returns>
        public async Task<List<ExcelRegisterUserModel>> RegisterListUsersFromExcel(List<ExcelRegisterUserModel> users)
        {
            var failedUsers = new List<ExcelRegisterUserModel>();

            foreach (var user in users)
            {
                var newUser = new ApplicationUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed
                };
                var result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Created user {newUser.UserName} ");
                }
                else
                {
                    _logger.LogInformation("--- Register UnSuccessful");
                    failedUsers.Add(user);
                }
            }

            return failedUsers;
        }
    }
}
