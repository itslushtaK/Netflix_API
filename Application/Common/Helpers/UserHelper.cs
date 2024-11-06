using Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Errors.Model;
using System.Security.Claims;

namespace CodeInvention.ScalingUp.Application.Common.Helpers
{
    public static class UserHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static IConfiguration _configuration;
        private static readonly string OrganizationIdHeaderKey = "Organization-Id";

        public static void Configure(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public static int GetUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext.User;

            if (user?.Identity != null && user.Identity.IsAuthenticated)
            {
                var claimsPrincipal = httpContext.User;
                var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

                if (int.TryParse(userIdClaim?.Value, out var userId))
                {
                    return userId;
                }
                else
                    throw new UnauthorizedException("User is not authenticated.");
            }
            else
                throw new UnauthorizedException("User is not authenticated.");
        }

        public static int TryGetOrganizationId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var headers = httpContext.Request.Headers;

            if (headers.TryGetValue(OrganizationIdHeaderKey, out var organizationIdHeaderValue) && int.TryParse(organizationIdHeaderValue.FirstOrDefault(), out int organizationId))
            {
                return organizationId;
            }

            throw new ForbiddenException("Organization Id is required.");
        }

        public static string GetFullName()
        {
            if (_httpContextAccessor.HttpContext.User.Identity == null || !_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                throw new UnauthorizedException("User is not authenticated.");

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var fullName = $"{claims.Where(x => x.Type == ClaimTypes.GivenName)?.FirstOrDefault()?.Value} {claims.Where(x => x.Type == ClaimTypes.Surname)?.FirstOrDefault()?.Value}";

            return fullName;
        }

        public static string GetUserAgent()
        {
            var userAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();

            return userAgent;
        }

        public static string GetGeoLocation()
        {
            var ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv6().ToString();

            return ip;
        }

        public static string GetModuleURL(string url)
        {
            var customer = _configuration[ConfigurationConstant.ApplicationDomainsCustomer];

            return url.Replace("{{Customer}}", customer);
        }
        public static IList<Claim> PrepareClaims(IdentityUser user , string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.AuthenticationMethod, user.TwoFactorEnabled ? "mfa" : "pwd")
            };

            return claims;
        }
    }
}