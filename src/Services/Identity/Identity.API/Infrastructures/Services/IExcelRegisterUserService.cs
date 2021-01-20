using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Infrastructures.Models.ExcelRegisterUserModel;

namespace Identity.API.Infrastructures.Services
{
    public interface IExcelRegisterUserService
    {
        Task<List<ExcelRegisterUserModel>> RegisterListUsersFromExcel(List<ExcelRegisterUserModel> users);
    }
}
