using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Frontend.Services
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly AuthService _authService; // <<< Inject AuthService

        // Inject AuthService instead of HttpClient for this option
        public ApiAuthenticationStateProvider(AuthService authService)
        {
            _authService = authService;
            // Subscribe to the AuthService's state change event
            _authService.OnStateChanged += AuthStateChanged; // <<< Subscribe
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity(); // Start with empty identity

            // Check the state directly from AuthService
            if (_authService.CurrentUserRole != null)
            {
                // If a role exists, create an authenticated identity
                // You NEED a Name claim for Identity.IsAuthenticated to be true
                var claims = new List<Claim>
                {
                    // Use a placeholder Name or retrieve actual username if AuthService stores it
                    new Claim(ClaimTypes.Name, $"UserRole_{_authService.CurrentUserRole.Value}"),
                    // Add the Role claim based on AuthService state
                    new Claim(ClaimTypes.Role, _authService.CurrentUserRole.Value.ToString())
                };
                identity = new ClaimsIdentity(claims, "CustomAuthService"); // Indicate authentication type
            }

            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        // Method called when AuthService notifies its state changed (login/logout)
        private void AuthStateChanged()
        {
            // Notify Blazor's authorization components that the state has changed
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // Unsubscribe when the provider is disposed
        public void Dispose()
        {
            _authService.OnStateChanged -= AuthStateChanged; // <<< Unsubscribe
        }
    }
}
