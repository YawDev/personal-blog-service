using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PersonalBlog.Core.BusinessContext;
using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Models.BusinessModels;

namespace PersonalBlog.Infrastructure.ThirdPartyServices.EmailProviders
{
    public class SmtpEmailProvider(IOptions<SmtpOptions> options) : IEmailProvider
    {
        private readonly SmtpOptions _settings = options.Value;

        public async Task<EmailSendResult> DispatchEmailAsync(EmailMessage message)
        {

            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            mime.To.Add(new MailboxAddress(message.ToName ?? message.ToEmail, message.ToEmail));
            mime.Subject = message.Subject;

            mime.Body = new BodyBuilder
            {
                HtmlBody = message.HtmlBody,
                TextBody = message.PlainTextBody
            }.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // STARTTLS on 587 is the common case; use SslOnConnect for implicit TLS on 465.
                await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

                var response = await client.SendAsync(mime);
                await client.DisconnectAsync(quit: true);

                return new EmailSendResult { Succeeded = true, ProviderResponse = response };
            }
            catch (Exception ex)
            {
                return new EmailSendResult { Succeeded = false, Error = ex.Message };
            }

        }
    }

}