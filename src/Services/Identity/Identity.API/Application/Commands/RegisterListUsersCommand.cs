using System.Collections.Generic;
using Identity.API.Infrastructures.Models.ExcelRegisterUserModel;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Identity.API.Application.Commands
{
    public class RegisterListUsersCommand : IRequest<List<ExcelRegisterUserModel>>
    {
        public RegisterListUsersCommand(List<ExcelRegisterUserModel> users)
        {
            Users = users;
        }

        public List<ExcelRegisterUserModel> Users { get; set; }
    }
}
