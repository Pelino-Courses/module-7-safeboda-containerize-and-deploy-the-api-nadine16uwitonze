using System.Net.Http;
using System.Net.Http.Json;
using SafeBoda.Application.DTOs;
using SafeBoda.Admin.Models;

namespace SafeBoda.Admin.Services
{
    public class TripService
    {
        private readonly HttpClient _http;

        public TripService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TripDto>> GetTripsAsync()
        {
            return await _http.GetFromJsonAsync<List<TripDto>>("api/trips")
                   ?? new List<TripDto>();
        }

        public async Task<TripDto?> GetTripByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<TripDto>($"api/trips/{id}");
        }

        public async Task<TripDto?> CreateTripAsync(TripDto trip)
        {
            var response = await _http.PostAsJsonAsync("api/trips", trip);
            return await response.Content.ReadFromJsonAsync<TripDto>();
        }

        public async Task UpdateTripAsync(Guid id, TripDto trip)
        {
            await _http.PutAsJsonAsync($"api/trips/{id}", trip);
        }

        public async Task DeleteTripAsync(Guid id)
        {
            await _http.DeleteAsync($"api/trips/{id}");
        }
    }
}