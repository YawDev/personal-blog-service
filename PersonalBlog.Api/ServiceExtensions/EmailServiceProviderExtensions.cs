using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Infrastructure.ThirdPartyServices.EmailProviders;
using PersonalBlog.Models.BusinessModels;
using PersonalBlog.Models.Enums;

namespace PersonalBlog.Api.ServiceExtensions
{
    public static class EmailServiceProviderExtensions
    {
        public static IServiceCollection AddEmailProvider(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SmtpOptions>(config.GetSection("EmailSettings:Smtp"));
            services.Configure<ResendOptions>(config.GetSection("EmailSettings:Resend"));

            var provider = config.GetValue<EmailProvider>("EmailSettings:Provider"); // parses "Resend" -> EmailProvider.Resend
            return provider switch
            {
                EmailProvider.Smtp   => services.AddScoped<IEmailProvider, SmtpEmailProvider>(),
                EmailProvider.Resend => services.AddScoped<IEmailProvider, ResendEmailProvider>(),
                _ => throw new InvalidOperationException($"Unknown email provider '{provider}'.")
            };
        }

    }
}