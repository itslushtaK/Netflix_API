using FluentValidation.Validators;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Application.Common.Abstractions;
using Microsoft.Extensions.Configuration;
using Application.Features.Accounts;
using SendGrid.Helpers.Errors.Model;
using Microsoft.AspNetCore.Http;

namespace CodeInvention.ScalingUp.Application.Features.Accounts
{
    public class Login
    {
        public record Command(string email, string password) : IRequest<Response>;
        public class CommandHandler : IRequestHandler<Command, Response>
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly IConfiguration _configuration;
            private IMailService _mailService;
            private readonly IMediator _mediator;
            private readonly IApplicationDbContext _context;
            private readonly IHttpContextAccessor _icontext;

            public CommandHandler(
                UserManager<IdentityUser> userManager,

                IMailService mailService,
                IApplicationDbContext context,
                IMediator mediator, IConfiguration configuration,
                IHttpContextAccessor icontext)
            {
                _userManager = userManager;
                _context = context;
                _mailService = mailService;
                _mediator = mediator;
                _configuration = configuration;
                _icontext = icontext;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.email);

                if(user == null)
                {
                    throw new NotFoundException("User doesn't exist!");

                }
                var loginId = Guid.NewGuid().ToString();
                _icontext.HttpContext.Session.SetString("loginId", loginId);

                var result = await _mediator.Send(new GenerateToken.Command(user));

                if (!user.EmailConfirmed)
                {
                    return new Response("Email not confirmed. Please confirm your email to proceed.", null, null);

                }

                //await _mailService.SendEmailAsync(request.email, "New login", "<h1>Hey! new login to your account noticed</h1><p>New login to your account at</p> " + "<h1>" + DateTime.Now + "</h1>");

                return new Response(
                    user.Email,
                    loginId,
                    result
                    );
            }
        }
        public record Response(
            string? email,
            string loginId,
            GenerateToken.Response result
           );
      

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(SignInManager<IdentityUser> signInManager , UserManager<IdentityUser> userManager)
            {
                ClassLevelCascadeMode = CascadeMode.Stop;
                RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(x => x.email)
                    .NotEmpty()
                    .WithMessage("Email is required!")
                    .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                    .WithMessage("Provide a valid email!")
                    .Must((email) =>
                    {
                        return signInManager.UserManager.Users.Any(x => x.Email == email.ToLower());
                    })
                    .WithMessage("Account not found with this email!");
                RuleFor(x => x.password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MustAsync(async (request, password, cancellationToken) =>
            {
                var user = await userManager.FindByEmailAsync(request.email);
                if (user != null)
                {
                    return await userManager.CheckPasswordAsync(user, password); 
                }
                return false; 
            })
            .WithMessage("Wrong password! Try again!");
            }
        }
    }
}