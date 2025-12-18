using SafeBoda.Admin;
using SafeBoda.Admin.Services;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:5106/") });


builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<RegisterService>();

var host = builder.Build();

await host.RunAsync();
builder.Services.AddScoped<TripService>();
builder.Services.AddScoped<RiderService>();
builder.Services.AddScoped<DriverService>();