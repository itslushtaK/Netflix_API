//using MediatR;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.WebUtilities;
//using System.Text;

//namespace Application.Features.Accounts
//{
//    public class ConfirmEmail
//    {
//        public record Command(string id, string token) : IRequest;

//        public class CommandHandler : IRequestHandler<Command>
//        {
//            private readonly UserManager<IdentityUser> _userManager;
//            public CommandHandler(UserManager<IdentityUser> userManager)
//            {
//                _userManager = userManager;
//            }

//            public async Task Handle(Command request, CancellationToken cancellationToken)
//            {
//                var user = await _userManager.FindByIdAsync(request.id);

//                if (user == null)
//                    return NotFound("User not found.");
//                var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
//                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
//                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

//                if (result.Succeeded)
//                {
//                    return Redirect($"{_configuration["AppUrl"]}/confirmemail.html");
//                }
//                return BadRequest("Email confirmation failed.");
//            }
//        }
//    }
//}
