using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Dtos.ResponseDtos;

namespace PersonalBlog.Core.Interfaces.Business
{    
    public interface IEmailService
    {
        public Task<SendEmailResponseDTO> SendEmailAsync(SendEmailDTO sendEmailDTO);
    }
}