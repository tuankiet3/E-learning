using E_learning.DTO.Zoom;
using E_learning.Model.Zoom;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace E_learning.Services
{
    public class ZoomService : IZoomService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public ZoomService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        public async Task<ZoomMeetingResponse> CreateMeetingAsync(CreateMeetingDTO meetingDTO, string email)
        {
            var accessToken = await GetZoomAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var jsonContent = JsonSerializer.Serialize(meetingDTO, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"https://api.zoom.us/v2/users/{email}/meetings", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create Zoom meeting: {error}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var meetingResponse = JsonSerializer.Deserialize<ZoomMeetingResponse>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return meetingResponse;
        }

        private async Task<string> GetZoomAccessTokenAsync()
        {
            const string cacheKey = "ZoomAccessToken";

            if (_memoryCache.TryGetValue(cacheKey, out string accessToken))
            {
                return accessToken;
            }
            var clientId = _configuration["Zoom:ClientId"];
            var clientSecret = _configuration["Zoom:ClientSecret"];
            var accountId = _configuration["Zoom:AccountId"];

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");

            // Tạo Basic Authentication header
            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "account_credentials" },
                { "account_id", accountId }
            });

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var tokenData = JsonDocument.Parse(responseBody).RootElement;
            accessToken = tokenData.GetProperty("access_token").GetString();
            var expiresIn = tokenData.GetProperty("expires_in").GetInt32();

            // Lưu token vào cache, trừ đi 5 phút để đảm bảo an toàn
            _memoryCache.Set(cacheKey, accessToken, TimeSpan.FromSeconds(expiresIn - 300));

            return accessToken;
        }
    }
}
