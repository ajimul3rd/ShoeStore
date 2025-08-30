using Blazored.LocalStorage;
using ShoeStore.Servicess.Impl;
using ShoeStore.Shared.Dto;
using ShoeStore.Auth;

namespace ShoeStore.Components.ApiService
{
    public class ApiServices
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly IDataSerializer? _DataSerializer;
        private readonly IAuthHeaderService _authHeaderService;

        public ApiServices(HttpClient httpClient, ILocalStorageService localStorage, IDataSerializer? DataSerializer, IAuthHeaderService authHeaderService)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _DataSerializer = DataSerializer;
            _authHeaderService = authHeaderService;
        }      

        // Register Client
        public async Task<AuthResponse> RegisterClientAsync(RegisterModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register-client", model);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse == null)
            {
                throw new InvalidOperationException("Failed to parse the authentication response.");
            }
            return authResponse;
        }

        // Register Admin
        public async Task<AuthResponse> RegisterAdminAsync(RegisterModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register-admin", model);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse == null)
            {
                throw new InvalidOperationException("Failed to parse the authentication response.");
            }
            return authResponse;
        }

        // Login
        public async Task<AuthResponse> LoginAsync(LoginModel login)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", login);
            if (response.IsSuccessStatusCode)
            {
                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (authResponse == null)
                {
                    throw new InvalidOperationException("Failed to parse the login response.");
                }
                return authResponse;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ApplicationException($"Login failed: {errorContent}");
            }
        }

        // Refresh token
        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenModel model)
        {
           
            await _authHeaderService.AddAuthHeaderAsync(_httpClient);
            var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", model);
            response.EnsureSuccessStatusCode();

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (authResponse == null)
            {
                throw new InvalidOperationException("Failed to parse the refresh token response.");
            }
            return authResponse;
        }

        // Logout
        public async Task LogoutAsync()
        {
            await _authHeaderService.AddAuthHeaderAsync(_httpClient);
            var response = await _httpClient.PostAsync("api/auth/logout", null);
            response.EnsureSuccessStatusCode();
        }

        // Update User
        public async Task UpdateUserAsync(int id, UsersDto dto)
        {
            await _authHeaderService.AddAuthHeaderAsync(_httpClient);
            var response = await _httpClient.PutAsJsonAsync($"api/auth/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        // Delete User
        public async Task DeleteUserAsync(int id)
        {
            await _authHeaderService.AddAuthHeaderAsync(_httpClient);
            var response = await _httpClient.DeleteAsync($"api/auth/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
