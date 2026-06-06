using PersonalBlog.Core.BusinessContext;
using PersonalBlog.Models.BusinessModels;

namespace PersonalBlog.Core.Interfaces.Business
{
    public interface IEmailProvider
    {
        Task<EmailSendResult> DispatchEmailAsync(EmailMessage message);
    }
}