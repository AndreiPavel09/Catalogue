using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json; 
using Microsoft.JSInterop;
using static Frontend.Services.AuthService;

namespace Frontend.Services
{
    
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly IJSRuntime _jsRuntime; 
        private readonly AuthService _authService; 

        private const string TokenKey = "authToken";
        private const string UserInfoKey = "userInfo"; 

        private readonly AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, AuthService authService)
        {
            _jsRuntime = jsRuntime;
            _authService = authService;
            _authService.AuthenticationStateChanged += HandleAuthenticationStateChanged;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await GetFromStorage<string>(TokenKey);
                var userInfo = await GetFromStorage<UserInfoModel>(UserInfoKey);

                if (string.IsNullOrWhiteSpace(token) || userInfo == null || string.IsNullOrWhiteSpace(userInfo.Role))
                {
                    return _anonymous;
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
                    new Claim(ClaimTypes.Name, userInfo.Username ?? string.Empty),
                    new Claim(ClaimTypes.Role, userInfo.Role)
                };
                var identity = new ClaimsIdentity(claims, authenticationType: "localStorageAuth");
                var principal = new ClaimsPrincipal(identity);

                return new AuthenticationState(principal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex}");
                return _anonymous; 
            }
        }

        private void HandleAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        private async Task<T?> GetFromStorage<T>(string key) where T : class
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
                return json == null ? null : JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex) { Console.WriteLine($"Error GetFromStorage '{key}' in AuthProvider: {ex.Message}"); return null; }
        }

        public void Dispose()
        {
            if (_authService != null)
            {
                _authService.AuthenticationStateChanged -= HandleAuthenticationStateChanged;
            }
            GC.SuppressFinalize(this);
        }
    }
}