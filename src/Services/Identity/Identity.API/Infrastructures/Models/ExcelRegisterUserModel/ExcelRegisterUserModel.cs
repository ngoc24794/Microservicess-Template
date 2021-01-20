using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Infrastructures.Models.ExcelRegisterUserModel
{
    public class ExcelRegisterUserModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; } = false;
    }
}
