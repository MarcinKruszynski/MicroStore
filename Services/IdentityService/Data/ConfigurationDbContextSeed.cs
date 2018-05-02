using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityService;

namespace IdentityService.Data
{
    public class ConfigurationDbContextSeed
    {
        public async Task SeedAsync(ConfigurationDbContext context,IConfiguration configuration)
        {
            var clientUrls = new Dictionary<string, string>();
            clientUrls.Add("Spa", configuration.GetValue<string>("SpaClient"));

            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients(clientUrls))
                {
                    await context.Clients.AddAsync(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    await context.IdentityResources.AddAsync(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var api in Config.GetApiResources())
                {
                    await context.ApiResources.AddAsync(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
