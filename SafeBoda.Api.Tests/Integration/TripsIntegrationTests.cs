using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SafeBoda.Infrastructure.Data; 
using SafeBoda.Api.Models;
using SafeBoda.Core.Identity;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace SafeBoda.Api.Tests.Integration
{
    public class TripsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TripsIntegrationTests(CustomWebApplicationFactory factory)
        {
            
            _client = factory.CreateClient();

            
            using (var scope = factory.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var db = services.GetRequiredService<AppDbContext>();
                
                db.Database.EnsureCreated();

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var email = "nadine.uwitonze2@gmail.com";
                var password = "Nadine123!";

                if (!roleManager.RoleExistsAsync("ADMIN").GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new IdentityRole("ADMIN")).GetAwaiter().GetResult();
                }

                var existing = userManager.FindByEmailAsync(email).GetAwaiter().GetResult();
                if (existing == null)
                {
                    var appUser = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        FullName = "Nadine UWITONZE"
                    };

                    var result = userManager.CreateAsync(appUser, password).GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(appUser, "ADMIN").GetAwaiter().GetResult();
                    }
                }
            }

            
            AuthenticateAsAdmin().GetAwaiter().GetResult();
        }

        private async Task AuthenticateAsAdmin()
        {
            var adminCredentials = new
            {
                Email = "nadine.uwitonze2@gmail.com",
                Password = "Nadine123!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", adminCredentials);

            response.EnsureSuccessStatusCode();

            var wrapper = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
           
            var token = wrapper != null && wrapper.TryGetValue("token", out var t) ? t : await response.Content.ReadAsStringAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task GetTrips_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/trips");
            response.EnsureSuccessStatusCode();
        }
    }
}
