using Application.Common.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Application.Features.Accounts
{
    public class ForgetPassword
    {

        public record Command(string email) : IRequest<Response>;

        public class CommandHandler : IRequestHandler<Command, Response>
        {
            private readonly IConfiguration _configuration;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly IMailService _mailService;

            public CommandHandler(UserManager<IdentityUser> userManager, IConfiguration configuration, IMailService mailService)
            {
                _userManager = userManager;
                _configuration = configuration;
                _mailService = mailService; 
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.email);

                if (user == null)
                {
                    return new Response("User does not exist.");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Encoding.UTF8.GetBytes(token);
                var validToken = WebEncoders.Base64UrlEncode(encodedToken);

                string resetUrl = $"{_configuration["AppUrl"]}/ResetPassword?email={request.email}&token={validToken}";

                try
                {
                    await _mailService.SendEmailAsync(request.email, "Reset Password",
                        $"<h1>Follow the instructions to reset your password</h1>" +
                        $"<p>To reset your password, <a href='{resetUrl}'>click here</a></p>");
                    return new Response("Password reset email sent successfully.");
                }
                catch (Exception ex)
                {
                    return new Response($"An error occurred while sending the email: {ex.Message}");
                }
            }
        }

        public record Response(string message);
    }
}
