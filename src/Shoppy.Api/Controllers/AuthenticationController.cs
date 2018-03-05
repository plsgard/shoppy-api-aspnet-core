using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shoppy.Application.Authentication.Dtos;
using Shoppy.Core.Users;

namespace Shoppy.Api.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody]LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Login);
            if (user == null)
                return BadRequest();

            var result = await _signInManager.CheckPasswordSignInAsync
                (user, model.Password, true);

            if (!result.Succeeded)
                return Unauthorized();

            var claims = new[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Login),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, model.Login)
                    };

            var token = new JwtSecurityToken
            (
                _configuration["Auth:Token:Issuer"],
                _configuration["Auth:Token:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(60),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(_configuration["Auth:Token:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );

            return Ok(new TokenDto { ExpirationDate = token.ValidTo, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
