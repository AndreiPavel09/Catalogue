using System.Net.Http.Json;
using Frontend.Models;
using Microsoft.AspNetCore.Components;

namespace Frontend.Services
{
    public class RoleLoginResponse
    {
        public bool IsSuccess { get; set; }
        public UserRole? Role { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        public UserRole? CurrentUserRole { get; private set; } = null;
        public event Action? OnStateChanged;


        public AuthService(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager= navigationManager;  
        }

        public async Task<string?> Login(LoginRequest loginRequest)
        {
            string? errorMessage = null;
            CurrentUserRole = null;

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);

                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RoleLoginResponse>();
                    if(result!=null && result.IsSuccess && result.Role.HasValue)
                    {
                        string targetUrl;
                        CurrentUserRole = result.Role.Value;
                        NotifyStateChanged();
                        
                        return null;
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
            NotifyStateChanged();
            return errorMessage;
        }
        public void Logout()
        {
            CurrentUserRole = null; 
            NotifyStateChanged();
            _navigationManager.NavigateTo("/login"); 
        }
        private void NotifyStateChanged() => OnStateChanged?.Invoke();

    }
}
