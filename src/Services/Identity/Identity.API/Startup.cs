using System;
using Autofac;
using HealthChecks.UI.Client;
using MediatR;
using Microservices.Core.EventBus.Abstractions;
using Microservices.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Steeltoe.Discovery.Client;
using System.Reflection;
using Identity.API.Application.Queries.Models;
using Identity.API.Configuration;
using Identity.API.Infrastructures;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.API
{
    public class Startup
    {
        #region Public Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion Public Constructors

        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion Public Properties

        #region Public Methods

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEventBus eventBus, IMediator mediator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDiscoveryClient();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
            });

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test1 Api v1");
            });

            /*app.UseCors();*/
            app.UseEventBus(eventBus);
        }

        /// <summary>
        /// Phương thức này được gọi sau <see cref="ConfigureServices(IServiceCollection)"/>
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
            //services.AddDbContext<ApplicationDbContext>(
            //    options => options.UseSqlServer(Configuration.GetConnectionString("ConnectionString")));

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            
            //Cấu hình IndentityServer4
            //Chạy tĩnh
            services.AddIdentityServer()
                .AddInMemoryClients(IdentityConfiguration.Clients)
                .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
                .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
                .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
                .AddTestUsers(IdentityConfiguration.TestUsers)
                .AddDeveloperSigningCredential();
            
            //Chạy từ database
            /*string connectionString = Configuration.GetConnectionString("SqlConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer(options =>
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
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    options.EnableTokenCleanup = true;
                })
                .AddDeveloperSigningCredential();*/
        }

        #endregion Public Methods
    }
}