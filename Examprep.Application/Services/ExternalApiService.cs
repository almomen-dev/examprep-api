using Microsoft.IdentityModel.Tokens;

namespace Examprep.Application.Services
{
    public class ExternalApiService
    {
      
        private readonly HttpClient _http;
        public ExternalApiService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("JsonPlaceholder");
        }

        public async Task<string> GetTodoAsync(int id)
        {
            var response = await _http.GetAsync($"todos/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }


    } 
}
