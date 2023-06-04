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

        public async Task<T?> HttpGetRequest<T>(string requestUri)
        {
            var response = await _httpClient.GetAsync(requestUri);
            //if (!response.IsSuccessStatusCode)
            //    return null;
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task HttpPostRequest<T>(string requestUri, T entity)
        {
            var jsonString = JsonSerializer.Serialize<T>(entity);
            var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUri, content);
            //if (!response.IsSuccessStatusCode)
            //    return;
        }

        public async Task<bool> HttpDeleteRequest<T>(string requestUri)
        {
            var response = await _httpClient.DeleteAsync(requestUri);
            if (!response.IsSuccessStatusCode)
                return false;
            return true;
        }

        public async Task HttpUpdateRequest<T>(string requestUri, T entity)
        {
            var jsonString = JsonSerializer.Serialize<T>(entity);
            var content = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(requestUri, content);
            //if (!response.IsSuccessStatusCode)
            //    return;
        }
    }
}
