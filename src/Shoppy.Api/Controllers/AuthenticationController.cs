using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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

        /// <summary>
        /// Generates an authentication token for the provided user login.
        /// </summary>
        /// <param name="model">Couple of login/password account infos.</param>
        /// <returns>A new authentication Bearer token.</returns>
        /// <response code="200">Returns the newly-generated token.</response>
        /// <response code="400">If the user account is invalid.</response>            
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Token([FromBody]LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Username);
            if (user == null)
                return BadRequest();

            var result = await _signInManager.CheckPasswordSignInAsync
                (user, model.Password, true);

            if (!result.Succeeded)
                return Unauthorized();

            var claims = new[]
            {
                        new Claim(JwtRegisteredClaimNames.Sub, model.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

            var token = new JwtSecurityToken
            (
                _configuration["Auth:Token:Issuer"],
                _configuration["Auth:Token:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(_configuration["Auth:Token:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );

            return Ok(new TokenDto { Id = token.Id, ExpirationDate = token.ValidTo, Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
