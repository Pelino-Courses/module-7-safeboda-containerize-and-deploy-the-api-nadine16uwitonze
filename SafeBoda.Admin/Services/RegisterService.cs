using System.Net.Http.Json;
using SafeBoda.Application.DTOs;
using System.Text.Json;

namespace SafeBoda.Admin.Services
{
    public class RegisterService
    {
        private readonly HttpClient _http;

        public RegisterService(HttpClient http) => _http = http;

        public async Task<RegisterResult> RegisterAsync(RegisterDto dto)
        {
            try
            {
                // Quick health check so we return a clear error if the API is down/unreachable
                bool healthOk = false;
                string healthError = string.Empty;

                // Try with the configured base address (HTTPS expected when Blazor runs over HTTPS)
                try
                {
                    using var healthCts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                    var healthResp = await _http.GetAsync("api/health", healthCts.Token);
                    if (healthResp.IsSuccessStatusCode) healthOk = true;
                    else healthError = "API health check failed (status " + ((int)healthResp.StatusCode).ToString() + ")";
                }
                catch (Exception ex)
                {
                    // Common causes: API not running, API listening on different port, or HTTPS cert not trusted (browser blocks fetch)
                    healthError = "Primary health check failed: " + ex.Message;
                }

                if (!healthOk)
                {
                    // Don't try HTTP fallback from the browser — mixed-content will block it when the site is served over HTTPS.
                    // Provide a clearer, actionable error message for developers.
                    var guidance = "API health check timed out or unreachable: " + healthError + ".\n" +
                                   "Possible causes: the API is not running, the API is listening on a different port, or the HTTPS dev certificate is not trusted.\n" +
                                   "Actions: 1) Start the API (dotnet run in SafeBoda.Api). 2) Confirm the API reports listening URLs (https://localhost:5106).\n" +
                                   "3) If the browser shows an SSL error, run: `dotnet dev-certs https --trust` and restart the API.\n" +
                                   "4) If the API uses a different port, update the Blazor HttpClient base address in SafeBoda.Admin/Program.cs.";

                    return new RegisterResult { Success = false, Errors = new List<string> { guidance } };
                }

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                var response = await _http.PostAsJsonAsync("api/auth/register", dto, cts.Token);

                if (response.IsSuccessStatusCode)
                    return new RegisterResult { Success = true };

                // Try to read errors from the response body (could be ModelState or plain message)
                var errors = new List<string>();

                try
                {
                    var body = await response.Content.ReadAsStringAsync(cts.Token);

                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        // Try parse as ProblemDetails / ModelState format or plain JSON with message
                        try
                        {
                            using var doc = JsonDocument.Parse(body);
                            var root = doc.RootElement;

                            // If it's an object with "errors" property (ModelState) extract values
                            if (root.TryGetProperty("errors", out var errorsElem) && errorsElem.ValueKind == JsonValueKind.Object)
                            {
                                foreach (var prop in errorsElem.EnumerateObject())
                                {
                                    foreach (var item in prop.Value.EnumerateArray())
                                    {
                                        errors.Add(item.GetString() ?? string.Empty);
                                    }
                                }
                            }
                            else if (root.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var item in root.EnumerateArray())
                                {
                                    if (item.ValueKind == JsonValueKind.String)
                                        errors.Add(item.GetString() ?? string.Empty);
                                }
                            }
                            else if (root.TryGetProperty("message", out var msg))
                            {
                                errors.Add(msg.GetString() ?? string.Empty);
                            }
                            else
                            {
                                // fallback: push whole body
                                errors.Add(body);
                            }
                        }
                        catch
                        {
                            errors.Add(body);
                        }
                    }
                }
                catch
                {
                    // ignore read errors
                }

                return new RegisterResult { Success = false, Errors = errors };
            }
            catch (OperationCanceledException)
            {
                return new RegisterResult { Success = false, Errors = new List<string> { "Request timed out." } };
            }
            catch (Exception ex)
            {
                return new RegisterResult { Success = false, Errors = new List<string> { "Request failed: " + ex.Message } };
            }
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result?.Token;
        }
    }

    public class RegisterResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}