using PersonalBlog.Core.BusinessContext.EmailTemplates;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.BusinessModels;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.BusinessContext
{
    public class EmailService(IEmailPostSendEventRepository emailPostSendEventRepository, IUserRepository userRepository, IEmailProvider emailProvider) : IEmailService
    {
        private readonly IEmailProvider _emailProvider = emailProvider;
        private readonly IEmailPostSendEventRepository _emailPostSendEventRepository = emailPostSendEventRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<SendEmailResponseDTO> SendEmailAsync(SendEmailDTO sendEmailDTO)
        {

            // If IdentityUserId is provided, we attempt to fetch the user. If not found, we throw an exception.
            ApplicationUser? existingUser = null;

            try
            {
                if (sendEmailDTO.IdentityUserId is not null)
                {
                    existingUser = await _userRepository.GetApplicationUserAsync(sendEmailDTO.IdentityUserId.Value) ?? throw new Exception("User not found");
                }


                // Build the share email (HTML + plain-text) from the template.
                var (subject, htmlBody, plainTextBody) = BlogShareEmailTemplate.Build(
                    sendEmailDTO.BlogShareLink,
                    existingUser?.UserName);

                
                if(string.IsNullOrEmpty(sendEmailDTO.BlogShareLink) || string.IsNullOrEmpty(sendEmailDTO.RecipientEmail))
                {
                    throw new Exception("Required fields missing to send email: 'Sharelink', 'RecipientEmail'");
                }

                // Call Email Provider to send email
                var emailSendResult = await _emailProvider.DispatchEmailAsync(new EmailMessage()
                {
                    ToEmail = sendEmailDTO.RecipientEmail,
                    ToName = sendEmailDTO.RecipientEmail,
                    Subject = subject,
                    HtmlBody = htmlBody,
                    PlainTextBody = plainTextBody,
                });


                // Log the email sending event in the database
                var (result, eventGuid)  = await TrackEmailEvent(sendEmailDTO, existingUser);
                
                // Return the result of the email sending operation
                return new SendEmailResponseDTO
                {
                    IsTriggered = emailSendResult.Succeeded,
                    EventGuid = eventGuid
                };

            }
            catch (Exception)
            {
                var (_, eventGuid)  = await TrackEmailEvent(sendEmailDTO, existingUser);

                return new SendEmailResponseDTO
                {
                    IsTriggered = false,
                    EventGuid = eventGuid,
                };
            }

        }
        
        private async Task<(bool result, Guid eventGuid)> TrackEmailEvent(SendEmailDTO sendEmailDTO, ApplicationUser? existingUser = null)
        {
            var (result, eventGuid) = await _emailPostSendEventRepository.CreateAsync(new Models.DatabaseModels.EmailPostSendEvent
            {
                PostId = sendEmailDTO.PostId,
                IdentityUserId = sendEmailDTO.IdentityUserId.HasValue ? existingUser?.Id : null,
                Recipient = sendEmailDTO.RecipientEmail,
                SentOn = DateTime.UtcNow
            });

            return (result > 0, eventGuid);
        }

    }
}