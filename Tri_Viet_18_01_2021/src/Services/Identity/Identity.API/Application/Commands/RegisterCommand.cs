using System.Runtime.Serialization;
using MediatR;

namespace Identity.API.Application.Commands
{
    /// <summary>
    /// Lệnh tạo mới đăng ký người dùng
    /// </summary>
    public class RegisterCommand : IRequest<bool>
    {
        #region Constructors

        public RegisterCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        #endregion

        #region Properties

        [DataMember] public string Email { get; set; }

        [DataMember] public string Password { get; set; }

        #endregion
    }
}