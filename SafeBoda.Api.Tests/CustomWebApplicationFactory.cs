using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using SafeBoda.Infrastructure.Data;

namespace SafeBoda.Api.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            
            builder.UseEnvironment("Testing");

            
            builder.ConfigureAppConfiguration((context, conf) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "TestJwtKeyForIntegrationTests_0123456789",
                    ["Jwt:Issuer"] = "SafeBodaTestIssuer",
                    ["Jwt:Audience"] = "SafeBodaTestAudience",
                    
                    ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:"
                };

                conf.AddInMemoryCollection(settings!);
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        }
    }
}