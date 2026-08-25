using FirebaseAdmin.Messaging;

namespace B2bOrder.Services.Common
{
    public interface IFCMService
    {
        Task<bool> SendAsync(string token, string title, string body);
        Task<bool> SendTopicAsync(string topic, string title, string body);
    }

    public class FCMService : IFCMService
    {
        public async Task<bool> SendAsync(string token, string title, string body)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            try
            {
                var message = new Message
                {
                    Token = token,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Data = new Dictionary<string, string>
                    {
                        { "type", "notice" }
                    }
                };

                var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return !string.IsNullOrWhiteSpace(result);
            }
            catch (Exception ex)
            {
                // 可在此處上記錄（Log）以利除錯，例如 _logger.LogError(ex, "FCM Send Error");
                return false;
            }
        }

        public async Task<bool> SendTopicAsync(string topic, string title, string body)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                return false;
            }

            try
            {
                var message = new Message
                {
                    Topic = topic,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    }
                };

                var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return !string.IsNullOrWhiteSpace(result);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}