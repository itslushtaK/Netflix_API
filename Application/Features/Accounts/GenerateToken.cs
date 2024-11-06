using Application.Common.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Application.Features.Accounts
{
    public class GenerateToken
    {
        public record Command(IdentityUser User) : IRequest<Response>;

        public class CommandHandler : IRequestHandler<Command, Response>
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly IConfiguration _configuration;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly IApplicationDbContext _context;

            public CommandHandler(
                UserManager<IdentityUser> userManager,
                IConfiguration configuration,
                IHttpContextAccessor contextAccessor,
                IApplicationDbContext context)
            {
                _userManager = userManager;
                _configuration = configuration;
                _contextAccessor = contextAccessor;
                _context = context;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = request.User;
                var userRoles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                //var claim = new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, user.Id),
                //    new Claim(ClaimTypes.Email, user.Email)
                //};

                //foreach (var role in userRoles)
                //{
                //    claim.Add(new Claim(ClaimTypes.Role, role));
                //}
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }


                var accessTokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Authentication:AccessTokenExpirationInMinutes"]));
                var refreshToken = TokenHelper.GenerateRefreshToken();
                var requestUrl = _contextAccessor.HttpContext.Request.Headers["Origin"].ToString();
                var accessToken = TokenHelper.GenerateJwt(
                    claims,
                    accessTokenExpiration,
                    secret: _configuration["Authentication:Secret"],
                    audience: _configuration["Authentication:Audience"],
                    issuer: _configuration["Authentication:Issuer"]
                );

                await _context.SaveChangesAsync(cancellationToken);

                return new Response(accessTokenExpiration, accessToken, refreshToken, "Bearer");
            }
        }

        public record Response(
            DateTime AccessTokenExpiration,
            string AccessToken,
            string RefreshToken,
            string Schema);
    }
}
