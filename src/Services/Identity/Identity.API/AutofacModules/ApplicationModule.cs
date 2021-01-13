using Autofac;
using FluentValidation;
using MediatR;
using Microservices.Core.Domain.Idempotency;
using Microservices.Core.Domain.SeedWork;
using Microservices.Core.EventBus.Abstractions;
using Microservices.Core.EventBus.CommandBus.Idempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Identity.API.AutofacModules
{
    public class ApplicationModule : Autofac.Module
    {
        #region Protected Methods

        protected override void Load(ContainerBuilder builder)
        {
            // Đăng ký lớp thi hành IRequestManager
            builder.RegisterType<RequestManager>().As<IRequestManager>()
                .InstancePerLifetimeScope();

            // Đăng ký tất cả các lớp thực hiện xác thực dữ liệu trong assembly này
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(IValidator<>))
                .InstancePerLifetimeScope();

            // Đăng ký tất cả các lớp xử lí sự kiện từ RabbitMQ trong assembly này
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .AsClosedTypesOf(typeof(IIntegrationEventHandler<>))
               .InstancePerLifetimeScope();

            builder.Register<DbContextCore>(context =>
            {
                var mediator = context.Resolve<IMediator>();
                var configuration = context.Resolve<IConfiguration>();
                return new DbContextCore(new DbContextOptionsBuilder<DbContextCore>().UseSqlServer(configuration["ConnectionString"]).Options, mediator);
            }).SingleInstance().InstancePerLifetimeScope();
        }

        #endregion Protected Methods
    }
}