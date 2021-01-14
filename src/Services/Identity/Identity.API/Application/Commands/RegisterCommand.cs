using System;
using System.Runtime.Serialization;
using MediatR;

namespace Identity.API.Application.Commands
{
    /// <summary>
    /// Lệnh tạo mới đăng ký người dùng
    /// </summary>
    public class RegisterCommand : IRequest<bool>
    {
        #region Private Fields

        #endregion Private Fields

        #region Public Constructors

        public RegisterCommand(string Name, string Email, string Password, string ConfirmPassword)
        {
            Email = Email;
            Name = Name;
            Password = Password;
            ConfirmPassword = ConfirmPassword;
        }

        #endregion Public Constructors

        #region Public Properties

        /*[DataMember] public DateTime DOB { get; private set; }*/

        [DataMember] public string Name { get; set; }

        [DataMember] public string Email { get; set; }

        [DataMember] public string Password { get; set; }

        [DataMember] public string ConfirmPassword { get; set; }

        #endregion Public Properties
    }
}