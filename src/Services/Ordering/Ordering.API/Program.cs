using Autofac.Extensions.DependencyInjection;
using Microservices.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Ordering.API
{
    public class Program
    {
        #region Public Methods

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    if (hostingContext.HostingEnvironment.IsProduction())
                    {
                        builder.AddConfigurationFromHost(
                            hostingContext.Configuration["ConsulHost"],
                            $"{hostingContext.HostingEnvironment.ApplicationName}/{hostingContext.HostingEnvironment.EnvironmentName}/appsettings.json");
                    }
                });

        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build().Run();
        }

        #endregion Public Methods
    }
}