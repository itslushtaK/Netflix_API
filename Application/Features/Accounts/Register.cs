using Application.Common.Abstractions;
using FluentValidation.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Application.Features.Accounts
{
    public class Register
    {
        public record Command(string userName, string firstName, string lastName, string email, string password) : IRequest
        {
            public IdentityUser ToEntity() => new IdentityUser
            {
                UserName = userName,
                Email = email
            };
        }
        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly IMediator _mediator;
            private readonly IMailService _mailService;
            private readonly IConfiguration _configuration;
            private readonly RoleManager<IdentityRole> _roleManager;


            public CommandHandler(UserManager<IdentityUser> userManager, IMediator mediator, IMailService mailService , IConfiguration configuration , RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _mediator = mediator;
                _mailService = mailService;
                _configuration = configuration;
                _roleManager = roleManager;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                var user = request.ToEntity();

                var createUserResult = await _userManager.CreateAsync(user, request.password);
                var roles = await _userManager.GetRolesAsync(user);

                if (createUserResult.Succeeded)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    if (!userRoles.Contains("User"))
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    await _mediator.Send(new GenerateUserClaims.Command(user.Id));
                    
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedEmailToken = Encoding.UTF8.GetBytes(token);
                    var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                    string url = $"{_configuration["AppUrl"]}/api/v1/accounts/confirmemail?userId={user.Id}&token={validEmailToken}";
                    await _mailService.SendEmailAsync(user.Email, "Confirm your email", "<h1>Welcome to Netflix</h1>" +
                        $"<p>Please confirm your email be <a href='{url}'> Clicking here</p>");
                }
            }
            public class CommandValidator : AbstractValidator<Command>
            {
                public CommandValidator(UserManager<IdentityUser> userManager)
                {
                    RuleLevelCascadeMode = CascadeMode.Stop;

                    RuleFor(x => x.password)
                        .NotEmpty()
                        .WithMessage("Password is required!");

                    RuleFor(x => x.userName)
                        .NotEmpty()
                        .WithMessage("Username is required!");

                    RuleFor(x => x.email)
                        .NotEmpty()
                        .WithMessage("Email is required!")
                        .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                        .WithMessage("Email is not valid")
                        .Must((email) =>
                        {
                            return !userManager.Users.Any(x => x.Email == email.ToLower());
                        })
                        .WithMessage("Account is already registered!");
                }
            }
        }
    }
}
