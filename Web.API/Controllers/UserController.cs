using Application.Features.Accounts;
using CodeInvention.ScalingUp.Application.Features.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Web.API.Controllers
{
    [Route("api/v1/accounts")]
    public class UserController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        public UserController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(Register.Command command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login.Command command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("forgetpassword")]
        public async Task<IActionResult> ForgotPassword(ForgetPassword.Command command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return Redirect($"{_configuration["emailConfirmed"]}");
            }
            return BadRequest("Email confirmation failed.");
        }
    }
}
