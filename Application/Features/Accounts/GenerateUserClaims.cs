using Application.Common.Abstractions;
using CodeInvention.ScalingUp.Application.Common.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Accounts
{
    public class GenerateUserClaims
    {
        public record Command(string id) : IRequest;

        public class CommandHandler : IRequestHandler<Command>
        {
            private IApplicationDbContext _context;
            private readonly UserManager<IdentityUser> _userManager;

            public CommandHandler(IApplicationDbContext context, UserManager<IdentityUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task Handle(Command request, CancellationToken cancellationToken)
            {

                var user = await _userManager.FindByIdAsync(request.id.ToString());
                var userRoles = (await _userManager.GetRolesAsync(user)).Distinct().ToList();

                var claims = await _userManager.GetClaimsAsync(user);
               
                if (claims != null)
                    await _userManager.RemoveClaimsAsync(user, claims);

                await _userManager.AddClaimsAsync(user, UserHelper.PrepareClaims(user, userRoles.FirstOrDefault()));
            }
        }
    }
}
