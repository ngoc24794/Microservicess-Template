using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Ordering.API.Application.Commands
{
    /// <summary>
    /// Lớp xác thực dữ liệu cho lệnh <see cref="CreateOrderCommand"/>
    /// </summary>
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator(ILogger<CreateOrderCommandValidator> logger)
        {
            //RuleFor(command => command.City).NotEmpty();
            //RuleFor(command => command.Street).NotEmpty();
            //RuleFor(command => command.State).NotEmpty();
            //RuleFor(command => command.Country).NotEmpty();
            //RuleFor(command => command.ZipCode).NotEmpty();
            //RuleFor(command => command.CardNumber).NotEmpty().Length(12, 19);
            //RuleFor(command => command.CardHolderName).NotEmpty();
            //RuleFor(command => command.CardExpiration).NotEmpty().Must(d => d >= DateTime.UtcNow).WithMessage("Please specify a valid card expiration date");
            //RuleFor(command => command.CardSecurityNumber).NotEmpty().Length(3);
            //RuleFor(command => command.CardTypeId).NotEmpty();
            //RuleFor(command => command.OrderItems).Must(o => o.Any()).WithMessage("No order items found");

            logger.LogTrace("----- INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}