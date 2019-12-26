using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RadarrAPI.Database;
using Microsoft.EntityFrameworkCore;

namespace RadarrAPI.Util
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            var serviceScopeFactory = (IServiceScopeFactory)webHost.Services.GetService(typeof(IServiceScopeFactory));

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<DatabaseContext>();

                dbContext.Database.Migrate();
            }

            return webHost;
        }
    }
}
