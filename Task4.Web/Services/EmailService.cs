using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Task4.Web.Services
{
    public sealed class EmailService(IOptions<MailSettings> options)
    {
        private readonly MailSettings settings = options.Value;

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
        {
            var message = CreateMessage(toEmail, subject, htmlBody);
            using var client = new SmtpClient();

            await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(settings.Username, settings.Password, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        private MimeMessage CreateMessage(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(settings.FromName, settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = CreateBody(htmlBody).ToMessageBody();

            return message;
        }

        private static BodyBuilder CreateBody(string htmlBody)
        {
            return new BodyBuilder
            {
                HtmlBody = htmlBody
            };
        }
    }
}
