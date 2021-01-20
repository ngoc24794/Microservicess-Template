using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Infrastructures.Models.ExcelRegisterUserModel
{
    public class RegisterListUserResponse<T>
    {

        public string Msg { get; set; }

        public T Data { get; set; }

        public static RegisterListUserResponse<T> GetResult(string msg, T data = default(T))
        {
            return new RegisterListUserResponse<T>
            {
                Msg = msg,
                Data = data
            };
        }
    }
}
