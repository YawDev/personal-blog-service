using PersonalBlog.Core.BusinessContext;
using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Models.BusinessModels;

namespace PersonalBlog.Infrastructure.ThirdPartyServices.EmailProviders
{
    public class ResendEmailProvider : IEmailProvider
    {
        public Task<EmailSendResult> DispatchEmailAsync(EmailMessage message)
        {
            throw new NotImplementedException();
        }
    }


}