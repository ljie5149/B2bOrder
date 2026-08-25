using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace B2bOrder.Services.Common
{
    public interface ILineService
    {
        Task<bool> PushAsync(
            string userId,
            string message);
    }
    public class LineService : ILineService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public LineService(
            HttpClient httpClient,
            IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<bool> PushAsync(
            string userId,
            string message)
        {
            try
            {
                var token =
                    _config["Line:ChannelAccessToken"];

                var body = new
                {
                    to = userId,

                    messages = new[]
                    {
                        new
                        {
                            type = "text",
                            text = message
                        }
                    }
                };

                var request =
                    new HttpRequestMessage(
                        HttpMethod.Post,
                        "https://api.line.me/v2/bot/message/push");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);

                request.Content =
                    new StringContent(
                        JsonSerializer.Serialize(body),
                        Encoding.UTF8,
                        "application/json");

                var response =
                    await _httpClient.SendAsync(
                        request);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}