using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace B2bOrder.Services.Common
{
    public interface IEmailService
    {
        Task<bool> SendAsync(
            string to,
            string subject,
            string body);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(
            IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> SendAsync(string to, string subject, string body)
        {
            try
            {
                var host        = _config["Mail:Host"] ?? "smtp.gmail.com";
                var portStr     = _config["Mail:Port"];
                var port        = string.IsNullOrEmpty(portStr) ? 587 : Convert.ToInt32(portStr);
                var account     = _config["Mail:Account"];
                var password    = _config["Mail:Password"];
                var senderName  = _config["Mail:SenderName"] ?? "System";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, account));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = body };
                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                // 關鍵：MailKit 會自動根據 Port 587 選用 SecureSocketOptions.StartTls
                await client.ConnectAsync(host, port, SecureSocketOptions.Auto);

                await client.AuthenticateAsync(account, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}