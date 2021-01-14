using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Identity.API.Application.Commands
{
    /// <summary>
    ///     Lớp xác thực dữ liệu cho lệnh <see cref="RegisterCommand" />
    /// </summary>
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        #region Constructors

        public RegisterCommandValidator(ILogger<RegisterCommandValidator> logger)
        {
            // RuleFor(command => command.Email)
            //     .NotEmpty().WithMessage("Không được bỏ trống")
            //     .MaximumLength(256).WithMessage("Độ dài tối đa là 256 ký tự")
            //     .EmailAddress().WithMessage("Email không đúng định dạng");
            // RuleFor(command => command.Password)
            //     .NotEmpty().WithMessage("Không được bỏ trống")
            //     .MaximumLength(50).WithMessage("Độ dài tối đa là 50 ký tự")
            //     .MinimumLength(6).WithMessage("Độ dài tối thiểu là 6 ký tự");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }

        #endregion
    }
}