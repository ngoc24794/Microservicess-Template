using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Identity.API.Application.Queries.Models;
using Identity.API.Configuration;
using Identity.API.Infrastructures;
using IdentityServer4.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using MediatR;
using Microservices.Core.EventBus.Abstractions;
using Microservices.Core.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            InitializeDatabase(app);

            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseDiscoveryClient();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = _ => true,
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
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        }, new List<string>()
                    }
                });
            });

            // Cross-origin resource sharing
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            // Api Controllers
            services.AddControllers().AddNewtonsoftJson().AddFluentValidation();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());
            
            //Khai báo cookie
            services.AddHttpContextAccessor();

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
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["Identity:Authority"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                    options.RequireHttpsMetadata = false;
                });

            //Cấu hình IndentityServer4
            //Khai báo chuỗi kết nối
            var connectionString = Configuration["ConnectionString"];
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
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential();
        }

        #endregion

        #region InsertData to IDSV4 when running OneTime

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
            {
                if (serviceScope != null)
                {
                    serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                    var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                    context.Database.Migrate();
                    if (!context.Clients.Any())
                    {
                        foreach (var client in Config.Clients)
                        {
                            context.Clients.Add(client.ToEntity());
                        }

                        context.SaveChanges();
                    }

                    if (!context.IdentityResources.Any())
                    {
                        foreach (var resource in Config.IdentityResources)
                        {
                            context.IdentityResources.Add(resource.ToEntity());
                        }

                        context.SaveChanges();
                    }

                    if (!context.ApiScopes.Any())
                    {
                        foreach (var resource in Config.ApiScopes)
                        {
                            context.ApiScopes.Add(resource.ToEntity());
                        }

                        context.SaveChanges();
                    }
                }
            }
        }

        #endregion
    }
}