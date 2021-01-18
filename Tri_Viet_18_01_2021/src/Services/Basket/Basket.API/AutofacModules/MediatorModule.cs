using Autofac;
using MediatR;
using MediatR.Extensions.Autofac.DependencyInjection;
using Microservices.Core.Domain.Behaviors;
using System.Reflection;

namespace Basket.API.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        #region Protected Methods

        protected override void Load(ContainerBuilder builder)
        {
            // Đăng ký tất cả những thứ liên quan đến MediatR
            builder.RegisterMediatR(Assembly.GetExecutingAssembly());

            builder.RegisterGeneric(typeof(LoggingBehavior<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(ValidatorBehavior<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(TransactionBehaviour<,>))
                .As(typeof(IPipelineBehavior<,>))
                .InstancePerLifetimeScope();
        }

        #endregion Protected Methods
    }
}