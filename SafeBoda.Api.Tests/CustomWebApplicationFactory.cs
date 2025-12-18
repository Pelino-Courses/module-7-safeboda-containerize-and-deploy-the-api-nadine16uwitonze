using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeBoda.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;

namespace SafeBoda.Api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.UseEnvironment("Testing");


            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "TestJwtKeyForIntegrationTests_0123456789",
                    ["Jwt:Issuer"] = "SafeBodaTestIssuer",
                    ["Jwt:Audience"] = "SafeBodaTestAudience",


                    ["ConnectionStrings:DefaultConnection"] = "InMemoryDbForTesting"
                };

                config.AddInMemoryCollection(settings!);
            });

            builder.ConfigureServices(services =>
            {

                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }


                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // 🔧 Build the service provider
                var sp = services.BuildServiceProvider();


                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                }
            });
        }
    }
}
