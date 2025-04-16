using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json; // Adăugat
using Microsoft.JSInterop;
using static Frontend.Services.AuthService; // Adăugat

namespace Frontend.Services
{
    // ACESTA ESTE SCOPED
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly IJSRuntime _jsRuntime; // Injectează pt localStorage
        private readonly AuthService _authService; // Injectează pt a se abona la eveniment

        private const string TokenKey = "authToken"; // Trebuie să fie consistent cu AuthService
        private const string UserInfoKey = "userInfo"; // Trebuie să fie consistent cu AuthService

        private readonly AuthenticationState _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, AuthService authService)
        {
            _jsRuntime = jsRuntime;
            _authService = authService;
            // Abonează-te la evenimentul din AuthService
            _authService.AuthenticationStateChanged += HandleAuthenticationStateChanged;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Citește token și user info din localStorage
                var token = await GetFromStorage<string>(TokenKey);
                var userInfo = await GetFromStorage<UserInfoModel>(UserInfoKey); // Citește UserInfoModel

                if (string.IsNullOrWhiteSpace(token) || userInfo == null || string.IsNullOrWhiteSpace(userInfo.Role))
                {
                    // Dacă lipsește token SAU user info SAU rolul, consideră nelogat
                    return _anonymous;
                }

                // --- Aici ai putea adăuga validare token (expirare, etc.) dacă token-ul e JWT ---

                // Construiește ClaimsPrincipal pe baza datelor din localStorage
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
                    new Claim(ClaimTypes.Name, userInfo.Username ?? string.Empty),
                    new Claim(ClaimTypes.Role, userInfo.Role) // Folosește rolul string direct
                };
                var identity = new ClaimsIdentity(claims, authenticationType: "localStorageAuth");
                var principal = new ClaimsPrincipal(identity);

                return new AuthenticationState(principal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAuthenticationStateAsync: {ex}");
                return _anonymous; // Returnează anonim la eroare
            }
        }

        // Metoda care este apelată când AuthService notifică o schimbare
        private void HandleAuthenticationStateChanged()
        {
            // Notifică Blazor să ceară din nou starea de autentificare
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // --- Metode Helper pt localStorage (duplicate din AuthService pt decuplare - sau creează un serviciu separat pt Storage) ---
        private async Task<T?> GetFromStorage<T>(string key) where T : class
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
                return json == null ? null : JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex) { Console.WriteLine($"Error GetFromStorage '{key}' in AuthProvider: {ex.Message}"); return null; }
        }
        // --------------------------------------------------------------------------------------------------------------------

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