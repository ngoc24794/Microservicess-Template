using System;
using System.Linq;
using System.Reflection;
using Autofac;
using HealthChecks.UI.Client;
using Identity.API.Application.Queries.Models;
using Identity.API.Configuration;
using Identity.API.Extensions;
using Identity.API.Infrastructures;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using MediatR;
using Microservices.Core.EventBus.Abstractions;
using Microservices.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Steeltoe.Discovery.Client;

namespace Identity.API
{
    public class Startup
    {
        #region Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Methods

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEventBus eventBus, IMediator mediator)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseDiscoveryClient();

            app.UseRouting();

            app.UseIdentityServerConfiguration();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate      = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test1 Api v1"); });

            /*app.UseCors();*/
            app.UseEventBus(eventBus);
        }

        /// <summary>
        ///     Phương thức này được gọi sau <see cref="ConfigureServices(IServiceCollection)" />
        /// </summary>
        /// <param name="builder">Autofac ContainerBuilder</param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Khởi tạo Serilog để ghi log
            builder.RegisterSerilog(Configuration);

            // Đăng ký tất cả các mô đun Autofac
            builder.RegisterAssemblyModules(Assembly.GetExecutingAssembly());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(x => services);

            // Service Discovery
            services.AddDiscoveryClient(Configuration);

            // RabbitMQ
            services.AddMessageBroker(Configuration);

            services.AddIntegrationEventLogService();

            // Swagger
            services.AddSwaggerGen();

            // Cross-origin resource sharing
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            // Api Controllers
            services.AddControllers().AddNewtonsoftJson();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

            //Cấu hình server để chạy Identity
            /*services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();*/
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Cấu hình IndentityServer4
            //Khai báo chuỗi kết nối
            var connectionString = Configuration["ConnectionString"];
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction.LoginUrl = "/Account/Login";
                    options.UserInteraction.LogoutUrl = "/Account/Logout";
                    options.Authentication = new AuthenticationOptions()
                    {
                        CookieLifetime = TimeSpan.FromHours(10), // ID server cookie timeout set to 10 hours
                        CookieSlidingExpiration = true
                    };
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();

            /*if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }*/
            // Adds IdentityServer
            /*var builder = services.AddIdentityServer()
                /*.AddTestUsers(TestUsers.Users)#1#
                .AddAspNetIdentity<ApplicationUser>()
                 .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                });*/

            /*builder.AddDeveloperSigningCredential();*/
        }
        
        #endregion

        #region InsertData to IDSV4 when running OneTime
        

        #endregion
    }
}