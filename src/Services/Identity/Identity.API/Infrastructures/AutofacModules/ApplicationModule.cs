using System.Reflection;
using Autofac;
using FluentValidation;
using Identity.API.Application.Queries.Models;
using Microservices.Core.Domain.Idempotency;
using Microservices.Core.Domain.SeedWork;
using Microservices.Core.EventBus.Abstractions;
using Microservices.Core.EventBus.CommandBus.Idempotency;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Module = Autofac.Module;

namespace Identity.API.Infrastructures.AutofacModules
{
    public class ApplicationModule : Module
    {
        #region Methods

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

            builder.Register<IDbContextCore>(context =>
            {
                var configuration = context.Resolve<IConfiguration>();
                return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer(configuration["ConnectionString"]).Options);
            }).SingleInstance().InstancePerLifetimeScope();
        }

        #endregion
    }
}