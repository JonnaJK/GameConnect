using GameConnect.Domain.Entities;
using System.Text.Json;

namespace GameConnect.DAL
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private const string Base_Address = "https://gameconnectforumapi.azurewebsites.net/api/";

        public HttpService()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(Base_Address)
            };
        }

        public async Task<T> HttpGetRequest<T>(string requestUri)
        {
            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }

        public async Task HttpPostRequest<T>(string requestUri, T entity)
        {
            var jsonString = JsonSerializer.Serialize<T>(entity);
            var response = await _httpClient.PostAsJsonAsync($"{requestUri}", jsonString);
            response.EnsureSuccessStatusCode();
        }
    }
}
