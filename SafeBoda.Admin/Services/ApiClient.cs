using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using SafeBoda.Admin.Models;
using SafeBoda.Application.DTOs;

namespace SafeBoda.Admin.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly AuthService _auth;

        public ApiClient(HttpClient http, AuthService auth)
        {
            _http = http;
            _auth = auth;
        }

        
        private async Task AddToken()
        {
            var token = await _auth.GetToken();

            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        
        public async Task<T?> GetAsync<T>(string url)
        {
            await AddToken();
            try
            {
                var response = await _http.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return default;

                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                return default;
            }
        }

        
        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            await AddToken();
            try
            {
                return await _http.PostAsJsonAsync(url, data);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        
        public async Task<HttpResponseMessage> PutAsync<T>(string url, T data)
        {
            await AddToken();
            try
            {
                return await _http.PutAsJsonAsync(url, data);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

       
        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            await AddToken();
            try
            {
                return await _http.DeleteAsync(url);
            }
            catch
            {
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

      

        
        public async Task<List<TripDto>> GetActiveTripsAsync()
        {
            var result = await GetAsync<List<TripDto>>("api/trips");
            return result ?? new List<TripDto>();
        }

        
        public async Task<List<DriverDto>> GetDriversAsync()
        {
            var result = await GetAsync<List<DriverDto>>("api/drivers");
            return result ?? new List<DriverDto>();
        }

       
        public async Task<List<RiderDto>> GetRidersAsync()
        {
            var result = await GetAsync<List<RiderDto>>("api/riders");
            return result ?? new List<RiderDto>();
        }

       
        public async Task<HttpResponseMessage> PostTripAsync(TripDto trip)
        {
            return await PostAsync("api/trips", trip);
        }
    }
}
