using Blazored.LocalStorage;
using ShoeStore.Servicess.Impl;
using System.Net.Http.Headers;

namespace ShoeStore.Servicess
{
    public class AuthHeaderService : IAuthHeaderService
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task AddAuthHeaderAsync(HttpClient httpClient)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                // Optionally, remove the header if token is null/empty
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}
