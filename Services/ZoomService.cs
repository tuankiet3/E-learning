using E_learning.DTO.Zoom;
using E_learning.Model.Zoom;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace E_learning.Services
{
    public class ZoomService : IZoomService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ZoomService> _logger;

        public ZoomService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ILogger<ZoomService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<ZoomMeetingResponse> CreateMeetingAsync(CreateMeetingDTO meetingDto, string email)
        {
            var accessToken = await GetZoomAccessTokenAsync();
            var client = _httpClientFactory.CreateClient("Zoom");

            var meetingObject = new
            {
                topic = meetingDto.Topic,
                type = 2, // Scheduled meeting
                start_time = meetingDto.StartTime,
                duration = meetingDto.Duration,
                timezone = meetingDto.Timezone,
                settings = new
                {
                    join_before_host = true,
                    mute_upon_entry = true,
                    participant_video = true,
                    host_video = true,
                    auto_recording = "none"
                }
            };

            var jsonContent = JsonSerializer.Serialize(meetingObject);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"users/{email}/meetings", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Zoom API Error on meeting creation. Status: {StatusCode}, Response: {Error}, RequestBody: {Body}", response.StatusCode, error, jsonContent);
                throw new Exception($"Failed to create Zoom meeting. Zoom API returned: {response.StatusCode} - {error}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ZoomMeetingResponse>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        private async Task<string> GetZoomAccessTokenAsync()
        {
            const string cacheKey = "ZoomAccessToken";
            if (_memoryCache.TryGetValue(cacheKey, out string accessToken))
            {
                _logger.LogInformation("Retrieved Zoom access token from cache.");
                return accessToken;
            }

            var clientId = _configuration["Zoom:ClientId"];
            var clientSecret = _configuration["Zoom:ClientSecret"];
            var accountId = _configuration["Zoom:AccountId"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(accountId))
            {
                _logger.LogError("Zoom configuration is missing in appsettings.json.");
                throw new InvalidOperationException("Zoom configuration (ClientId, ClientSecret, AccountId) is missing.");
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://zoom.us/oauth/token");

            var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "account_credentials" },
                { "account_id", accountId }
            });

            _logger.LogInformation("Requesting new Zoom access token for Account ID: {AccountId}", accountId);
            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get Zoom access token. Status: {StatusCode}, Response: {Response}", response.StatusCode, responseContent);
                throw new Exception($"Failed to get Zoom access token. Status: {response.StatusCode}, Error: {responseContent}");
            }

            var tokenData = JsonDocument.Parse(responseContent).RootElement;
            accessToken = tokenData.GetProperty("access_token").GetString();
            var expiresIn = tokenData.GetProperty("expires_in").GetInt32();

            _memoryCache.Set(cacheKey, accessToken, TimeSpan.FromSeconds(expiresIn - 300));
            _logger.LogInformation("Successfully obtained and cached new Zoom access token.");
            return accessToken;
        }

        public async Task<string> TestTokenGenerationAsync()
        {
            try
            {
                var token = await GetZoomAccessTokenAsync();
                return $"Token generated successfully. Length: {token?.Length ?? 0}";
            }
            catch (Exception ex)
            {
                return $"Token generation failed: {ex.Message}";
            }
        }
    }
}
