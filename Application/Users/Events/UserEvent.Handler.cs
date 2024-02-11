using Application.Common.Interfaces;
using Application.Common.Models;
using Common.Configurations;
using Common.Enumerations;
using Common.Helpers;
using Domain.Entities;
using Domain.Events;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Events
{
    public class UserEventHandler : INotificationHandler<CreatedEvent>, INotificationHandler<ActivatedEvent>
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;
        public UserEventHandler(IIdentityService identityService,
                                IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task Handle(CreatedEvent notification, CancellationToken cancellationToken)
        {
            var dbUser = notification.GetEntity<User>();
            if (dbUser != null && !dbUser.EmailConfirmed &&
                !dbUser.IsActive && dbUser.Activity == ActivityLog.Created)
            {
                await SendActivationLinkAsync(dbUser, cancellationToken);
            }
        }

        public async Task Handle(ActivatedEvent notification, CancellationToken cancellationToken)
        {
            var dbUser = notification.GetEntity<User>();
            if (dbUser != null)
            {
                switch (dbUser.Activity)
                {
                    case ActivityLog.Activated:
                        if (dbUser.IsActive)
                        {
                            await SendActivatedMailAsync(dbUser, cancellationToken);
                        }
                        break;
                    case ActivityLog.RequestReinvite:
                        if (!dbUser.EmailConfirmed)
                        {
                            await SendActivationLinkAsync(dbUser, cancellationToken);
                        }
                        break;
                    case ActivityLog.RequestPasswordReset:
                        await SendPasswordResetLinkAsync(dbUser, cancellationToken);
                        break;
                    case ActivityLog.PasswordReset:
                        await SendPasswordResetedMailAsync(dbUser, cancellationToken);
                        break;
                    case ActivityLog.Created:
                    default:
                        break;
                }
            }
        }

        private async Task SendPasswordResetedMailAsync(User dbUser, CancellationToken cancellationToken)
        {
            var loginLink = $"{dbUser.ClientUrl}";

            string emailTemplate = await FileHelper.ReadEmailTemplateAsync(EmailConfiguration.ACTIVATION_TEMPLATE, cancellationToken);

            // TODO: to be replaced.
            var imageBaseUrl = "";

            emailTemplate = emailTemplate.Replace("{{user_confirmation_icon}}", $"{imageBaseUrl}/assets/img/user.png");
            emailTemplate = emailTemplate.Replace("{{broker_logo}}", $"{imageBaseUrl}/assets/img/broker-logo.png");
            emailTemplate = emailTemplate.Replace("{{first_name}}", $"{dbUser.Profile.FirstName}");
            emailTemplate = emailTemplate.Replace("{{last_name}}", $"{dbUser.Profile.LastName}");
            emailTemplate = emailTemplate.Replace("{{user_login_name}}", $"{dbUser.UserName}");
            emailTemplate = emailTemplate.Replace("{{login_link}}", loginLink);

            var emailModel = new EmailModel
            {
                To = dbUser.Email,
                Body = emailTemplate,
                Subject = "Password Reset"
            };

            await _emailService.SendAsync(emailModel, cancellationToken);
        }

        private async Task SendPasswordResetLinkAsync(User dbUser, CancellationToken cancellationToken)
        {
            var token = await _identityService.GenerateResetPasswordTokenAsync(dbUser);
            var confirmationLink = $"{dbUser.ClientUrl}/accounts/reset-password?firstname={dbUser.Profile.FirstName}&lastname={dbUser.Profile.LastName}&email={dbUser.Email}&token={token}";

            string emailTemplate = await FileHelper.ReadEmailTemplateAsync(EmailConfiguration.REQUEST_RESET_PASSWORD_TEMPLATE, cancellationToken);

            // TODO: to be replaced.
            var imageBaseUrl = "";

            emailTemplate = emailTemplate.Replace("{{user_confirmation_icon}}", $"{imageBaseUrl}/assets/img/user.png");
            emailTemplate = emailTemplate.Replace("{{broker_logo}}", $"{imageBaseUrl}/assets/img/broker-logo.png");
            emailTemplate = emailTemplate.Replace("{{first_name}}", $"{dbUser.Profile.FirstName}");
            emailTemplate = emailTemplate.Replace("{{last_name}}", $"{dbUser.Profile.LastName}");
            emailTemplate = emailTemplate.Replace("{{reset_password_link}}", confirmationLink);

            var emailModel = new EmailModel
            {
                To = dbUser.Email,
                Body = emailTemplate,
                Subject = "Password Reset"
            };

            await _emailService.SendAsync(emailModel, cancellationToken);
        }

        private async Task SendActivationLinkAsync(User dbUser, CancellationToken cancellationToken)
        {
            var token = await _identityService.GenerateEmailConfirmationTokenAsync(dbUser);
            var confirmationLink = $"{dbUser.ClientUrl}/accounts/activate?firstname={dbUser.Profile.FirstName}&lastname={dbUser.Profile.LastName}&email={dbUser.Email}&token={token}";

            string emailTemplate = await FileHelper.ReadEmailTemplateAsync(EmailConfiguration.CONFIRMATION_TEMPLATE, cancellationToken);

            // TODO: to be replaced.
            var imageBaseUrl = "";

            emailTemplate = emailTemplate.Replace("{{user_confirmation_icon}}", $"{imageBaseUrl}/assets/img/user.png");
            emailTemplate = emailTemplate.Replace("{{broker_mechanics_logo}}", $"{imageBaseUrl}/assets/img/broker-logo.png");
            emailTemplate = emailTemplate.Replace("{{first_name}}", $"{dbUser.Profile.FirstName}");
            emailTemplate = emailTemplate.Replace("{{last_name}}", $"{dbUser.Profile.LastName}");
            emailTemplate = emailTemplate.Replace("{{user_login_name}}", $"{dbUser.UserName}");
            emailTemplate = emailTemplate.Replace("{{confirmation_link}}", confirmationLink);

            var emailModel = new EmailModel
            {
                To = dbUser.Email,
                Body = emailTemplate,
                Subject = "Account Confirmation"
            };
            
            await _emailService.SendAsync(emailModel, cancellationToken);
        }

        private async Task SendActivatedMailAsync(User dbUser, CancellationToken cancellationToken)
        {
            var loginLink = $"{dbUser.ClientUrl}";

            string emailTemplate = await FileHelper.ReadEmailTemplateAsync(EmailConfiguration.ACTIVATION_TEMPLATE, cancellationToken);

            // TODO: to be replaced.
            var imageBaseUrl = "";

            emailTemplate = emailTemplate.Replace("{{user_confirmation_icon}}", $"{imageBaseUrl}/assets/img/user.png");
            emailTemplate = emailTemplate.Replace("{{broker_logo}}", $"{imageBaseUrl}/assets/img/broker-logo.png");
            emailTemplate = emailTemplate.Replace("{{first_name}}", $"{dbUser.Profile.FirstName}");
            emailTemplate = emailTemplate.Replace("{{last_name}}", $"{dbUser.Profile.LastName}");
            emailTemplate = emailTemplate.Replace("{{user_login_name}}", $"{dbUser.UserName}");
            emailTemplate = emailTemplate.Replace("{{login_link}}", loginLink);

            var emailModel = new EmailModel
            {
                To = dbUser.Email,
                Body = emailTemplate,
                Subject = "Account Activated"
            };

            await _emailService.SendAsync(emailModel, cancellationToken);
        }
    }
}
