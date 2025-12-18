using Microsoft.JSInterop;

namespace SafeBoda.Admin.Services
{
    public class AuthService
    {
        private readonly IJSRuntime _js;

        public AuthService(IJSRuntime js)
        {
            _js = js;
        }

        
        public async Task SaveToken(string token)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", "authToken", token);
        }

        
        public async Task<string?> GetToken()
        {
            return await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
        }

        
        public async Task Logout()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }
    }
}