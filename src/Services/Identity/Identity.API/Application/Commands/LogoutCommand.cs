using System.Runtime.Serialization;
using MediatR;

namespace Identity.API.Application.Commands
{
    /// <summary>
    /// Lệnh tạo mới đăng ký người dùng
    /// </summary>
    public class LogoutCommand : IRequest<bool>
    {

        #region Properties

        [DataMember] public string returnUrl { get; set; }

        #endregion
    }
}