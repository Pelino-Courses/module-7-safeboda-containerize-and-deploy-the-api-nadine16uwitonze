using System.Net.Http;
using System.Net.Http.Json;
using SafeBoda.Application.DTOs;

namespace SafeBoda.Admin.Services
{
    public class DriverService
    {
        private readonly HttpClient _http;

        public DriverService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<DriverDto>> GetDriversAsync()
        {
            return await _http.GetFromJsonAsync<List<DriverDto>>("api/drivers")
                   ?? new List<DriverDto>();
        }

        public async Task<DriverDto?> GetDriverByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<DriverDto>($"api/drivers/{id}");
        }

        public async Task<DriverDto?> CreateDriverAsync(DriverDto driver)
        {
            var response = await _http.PostAsJsonAsync("api/drivers", driver);
            return await response.Content.ReadFromJsonAsync<DriverDto>();
        }

        public async Task UpdateDriverAsync(Guid id, DriverDto driver)
        {
            await _http.PutAsJsonAsync($"api/drivers/{id}", driver);
        }

        public async Task DeleteDriverAsync(Guid id)
        {
            await _http.DeleteAsync($"api/drivers/{id}");
        }
    }
}