using System.Net.Http.Json;
using System.Text.Json;
using Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Frontend.Services
{
    public class UserLoginResponse
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole? Role { get; set; } 
        public string? ErrorMessage { get; set; }
    }
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly IJSRuntime _jsRuntime;
        private const string TokenKey = "authToken";
        private const string UserInfoKey = "userInfo";

        public event Action? AuthenticationStateChanged;



        public AuthService(HttpClient httpClient, NavigationManager navigationManager ,IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _navigationManager= navigationManager;
            _jsRuntime = jsRuntime;

        }

        public async Task<string?> Login(LoginRequest loginRequest)
        {
            string? errorMessage = null;

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

                if(response.IsSuccessStatusCode)
                {

                    var result = await response.Content.ReadFromJsonAsync<UserLoginResponse>();
                    if(result!=null && result.IsSuccess && result.Role.HasValue)
                    {
                        await SetInStorage(TokenKey, result.Token);
                        var userInfo = new UserInfoModel
                        {
                            UserId = result.UserId,
                            Username = result.Username,
                            FirstName = result.FirstName,
                            LastName = result.LastName,
                            
                            Role = result.Role.Value.ToString() 
                        };
                        await SetInStorage(UserInfoKey, userInfo);

                        NotifyAuthenticationStateChangedInternal();

                        NavigateToRolePage(result.Role.Value);
                        return null; // Succes
                    }
                    else
                    {
                        errorMessage=result?.ErrorMessage ?? "Invalid response from server.";
                    }
                }
                else
                {
                    errorMessage = $"Login failed (HTTP {response.StatusCode})";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                errorMessage = "An error occurred during login.";
            }
            await ClearStorage();
            NotifyAuthenticationStateChangedInternal();
            return errorMessage;
        }
        public async Task Logout()
        {
            Console.WriteLine("AuthService: Logout called."); 
            await ClearStorage();
            NotifyAuthenticationStateChangedInternal(); 
            _navigationManager.NavigateTo("/login"); 
            try { await _httpClient.PostAsync("api/auth/logout", null); } catch { }
        }
        public async Task<string?> GetTokenAsync() => await GetFromStorage<string>(TokenKey);
        public async Task<UserInfoModel?> GetStoredUserInfoAsync() => await GetFromStorage<UserInfoModel>(UserInfoKey);

        private async Task SetInStorage<T>(string key, T value)
        {
            try { await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value)); }
            catch (Exception ex) { Console.WriteLine($"Error SetInStorage '{key}': {ex.Message}"); }
        }
        private async Task<T?> GetFromStorage<T>(string key) where T : class
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
                return json == null ? null : JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex) { Console.WriteLine($"Error GetFromStorage '{key}': {ex.Message}"); return null; }
        }
        private async Task ClearStorage()
        {
            await RemoveFromStorage(TokenKey);
            await RemoveFromStorage(UserInfoKey);
        }
        private async Task RemoveFromStorage(string key)
        {
            try { await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key); }
            catch (Exception ex) { Console.WriteLine($"Error RemoveFromStorage '{key}': {ex.Message}"); }
        }
        private void NavigateToRolePage(UserRole role) // Metodă ajutătoare pt navigare
        {
            string targetUrl = role switch
            {
                UserRole.Admin => "/admin",
                UserRole.Teacher => "/teacher",
                UserRole.Student => "/student",
                _ => "/"
            };
            _navigationManager.NavigateTo(targetUrl);
        }

        private void NotifyAuthenticationStateChangedInternal() => AuthenticationStateChanged?.Invoke();

        public class UserInfoModel
        {
            public int UserId { get; set; }
            public string? Username { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Role { get; set; } 
        }
    }
}
