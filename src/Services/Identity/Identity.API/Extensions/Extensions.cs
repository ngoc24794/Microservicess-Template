using System.Linq;
using Identity.API.Configuration;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.API.Extensions
{
    public static class Extensions
    {
        #region Methods

        /// <summary>
        /// Đọc và lưu các cấu hình từ <see cref="Config"/> vào cơ sở dữ liệu
        /// </summary>
        /// <param name="app"></param>
        public static void UseIdentityServerConfiguration(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                var clients = Config.Clients.Where(x => context.Clients.Any(o => x.ClientId == o.ClientId) == false).ToList();
                if (clients.Any())
                {
                    foreach (var client in clients) context.Clients.Add(client.ToEntity());
                    context.SaveChanges();
                }

                var identityResources = Config.IdentityResources.Where(x => context.IdentityResources.Any(o => x.Name == o.Name) == false).ToList();
                if (identityResources.Any())
                {
                    foreach (var resource in identityResources) context.IdentityResources.Add(resource.ToEntity());
                    context.SaveChanges();
                }

                var scopes = Config.ApiScopes.Where(x => context.ApiScopes.Any(o => x.Name == o.Name) == false).ToList();
                if (scopes.Any())
                {
                    foreach (var resource in scopes) context.ApiScopes.Add(resource.ToEntity());

                    context.SaveChanges();
                }
            }
        }

        #endregion
    }
}