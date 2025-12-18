using System.Net.Http;
using System.Net.Http.Json;
using SafeBoda.Application.DTOs;

namespace SafeBoda.Admin.Services
{
    public class RiderService
    {
        private readonly HttpClient _http;

        public RiderService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<RiderDto>> GetRidersAsync()
        {
            return await _http.GetFromJsonAsync<List<RiderDto>>("api/riders")
                   ?? new List<RiderDto>();
        }

        public async Task<RiderDto?> GetRiderByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<RiderDto>($"api/riders/{id}");
        }

        public async Task<RiderDto?> CreateRiderAsync(RiderDto rider)
        {
            var response = await _http.PostAsJsonAsync("api/riders", rider);
            return await response.Content.ReadFromJsonAsync<RiderDto>();
        }

        public async Task UpdateRiderAsync(Guid id, RiderDto rider)
        {
            await _http.PutAsJsonAsync($"api/riders/{id}", rider);
        }

        public async Task DeleteRiderAsync(Guid id)
        {
            await _http.DeleteAsync($"api/riders/{id}");
        }
    }
}